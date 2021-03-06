using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CkpServices.Interfaces;
using CkpDAL;
using CkpModel.Output;
using CkpDAL.Entities;
using CkpInfrastructure.Configuration;
using CkpServices.Helpers;
using Microsoft.EntityFrameworkCore.Storage;
using CkpServices.Processors.Interfaces;
using System.Data.Common;
using CkpServices.Processors;
using CkpServices.Processors.String;
using CkpDAL.Repository;
using CkpServices.Helpers.Providers;
using CkpInfrastructure.Providers.Interfaces;
using System;
using CkpServices.Processors.Module;

namespace CkpServices
{
    public class AccountService : IAccountService
    {
        private readonly BPFinanceContext _context;

        private readonly IKeyedProvider<int, float> _paymentInTimeDiscountProvider;

        private readonly IClientAccountProcessor _clientAccountProcessor;
        private readonly IAccountSettingsProcessor _accountSettingsProcessor;
        private readonly IOrderProcessor _orderProcessor;
        private readonly IOrderPositionProcessor _orderPositionProcessor;
        private readonly IOrderImProcessor _orderImProcessor;
        private readonly IPositionImProcessor _positionImProcessor;

        private readonly int[] _businessUnitIds;

        public AccountService(BPFinanceContext context, IOptions<AppSettings> appSettingsAccessor, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            _businessUnitIds = appParamsAccessor.Value.BusinessUnitIds;

            var basketBusinessUnitIdProvider = new BasketBusinessUnitIdProvider(_context);

            _paymentInTimeDiscountProvider = new PaymentInTimeDiscountProvider(appParamsAccessor.Value.BusinessUnitSettings);

            _clientAccountProcessor = new ClientAccountProcessor(
                _context,
                repository);
            _accountSettingsProcessor = new AccountSettingsProcessor(
                _context,
                repository);
            _orderProcessor = new OrderProcesor(
                _context,
                repository,
                appParamsAccessor.Value.BasketOrderDescription,
                appParamsAccessor.Value.ManagerId,
                basketBusinessUnitIdProvider);
            var rubricProcessor = new RubricProcessor(
                _context,
                repository);
            var graphicProcessor = new GraphicProcessor(
                _context,
                repository);
            _orderImProcessor = new OrderImProcessor(
                _context,
                repository);
            var stringProcessor = new StringProcessor(
                _context,
                repository);
            var moduleProcessor = new ModuleProcessor(
                _context,
                repository);
            var moduleMaketProcessor = new ModuleMaketProcessor(
                appSettingsAccessor.Value.OrderImFolderTemplate,
                appSettingsAccessor.Value.DatabaseName);
            _positionImProcessor = new PositionImProcessor(
                _context,
                repository,
                _orderImProcessor,
                stringProcessor,
                moduleProcessor,
                moduleMaketProcessor);
            _orderPositionProcessor = new OrderPositionProcessor(
                _context,
                repository,
                rubricProcessor,
                graphicProcessor,
                _positionImProcessor,
                appParamsAccessor.Value.BasketOrderDescription);
        }

        public void ApplyPaymentInTimeDiscount(int accountId)
        {
            var account = _context.Accounts
                .Include(ac => ac.AccountPositions)
                .Include(ac => ac.AccountOrders).ThenInclude(ao => ao.Order)
                .Single(ac => ac.Id == accountId);

            var discount = _paymentInTimeDiscountProvider.GetByValue(account.BusinessUnitId);

            if (discount == 0) return;

            var description = string.Format("Доп. скидка {0}%.", discount);

            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                // Удаляем старые позиции счёта
                foreach (var accountPosition in account.AccountPositions)
                    _clientAccountProcessor.DeleteAccountPosition(accountPosition, dbTran);

                foreach (var accountOrder in account.AccountOrders)
                {
                    var order = accountOrder.Order;
                    var orderPositions = _context.OrderPositions
                        .Include(op => op.Supplier).ThenInclude(op => op.Company)
                        .Include(op => op.Supplier).ThenInclude(op => op.City)
                        .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionEx)
                        .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                        .Include(op => op.PricePosition).ThenInclude(pp => pp.Unit)
                        .Where(op => op.OrderId == order.Id)
                        .ToList();

                    foreach (var orderPosition in orderPositions)
                    {
                        orderPosition.Discount += discount;

                        // Сохраняем позицию заказа
                        _orderPositionProcessor.UpdateOrderPosition(orderPosition, order.Id, dbTran);

                        // Если позиция заказа - не часть пакета
                        if (orderPosition.ParentOrderPositionId == null)
                        {
                            // находим её пакетные позиции
                            var packagePositions = orderPositions
                                .Where(op => op.ParentOrderPositionId == orderPosition.Id)
                                .ToList();

                            // Создаём позицию счёта
                            var accountPosition = _clientAccountProcessor
                                .CreateAccountPosition(account.Id, orderPosition, packagePositions, dbTran);
                        }
                    }

                    // Пересчитываем сумму заказа
                    order.Sum = (float)Math.Round(
                        orderPositions
                        .Sum(
                            op =>
                                op.Price.Value * op.GraphicPositions
                                    .Where(
                                        gp =>
                                            gp.ParenGraphicPositiontId == gp.Id &&
                                            op.ParentOrderPositionId == null)
                                    .Sum(gp => gp.Count) * (1 - op.Discount / 100)), 2);

                    order.AccountDescription = description;

                    _orderProcessor.UpdateOrder(order, dbTran);
                }

                // Пересчитываем сумму счёта
                account.Sum = (float)Math.Round(account.AccountOrders.Sum(o => o.Order.Sum), 2);
                account.Description = description;

                _clientAccountProcessor.UpdateClientAccout(account, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        public bool ExistsById(int accountId)
        {
            var account = _context.Accounts
                .SingleOrDefault(ac => ac.Id == accountId);

            return account != null;
        }

        #region Get

        public async Task<ActionResult<AccountInfo>> GetAccountByIdAsync(int accountId)
        {
            var account = await _context.Accounts
                .Include(ac => ac.LegalPerson)
                .Include(ac => ac.BusinessUnit.LegalPerson)
                .Include(ac => ac.AccountSettings.LegalPersonBank.Bank)
                .Where(ac => ac.Id == accountId)
                .Select(
                    ac =>
                        new AccountInfo
                        {
                            Id = ac.Id,
                            Number = ac.Number,
                            Date = ac.Date,
                            Nds = ac.Nds,
                            ClientLegalPerson =
                                new LegalPersonInfo
                                {
                                    Id = ac.LegalPersonId,
                                    Name = ac.LegalPerson.Name,
                                    Inn = ac.LegalPerson.Inn,
                                    Kpp = ac.LegalPerson.Kpp,
                                    Okpo = ac.LegalPerson.Okpo,
                                    LegalAddress = ac.LegalPerson.LegalAddress
                                },
                            SupplierLegalPerson =
                                new LegalPersonInfo
                                {
                                    Id = ac.BusinessUnit.LegalPersonId,
                                    Name = ac.BusinessUnit.LegalPerson.Name,
                                    Inn = ac.BusinessUnit.LegalPerson.Inn,
                                    Kpp = ac.BusinessUnit.LegalPerson.Kpp,
                                    Okpo = ac.BusinessUnit.LegalPerson.Okpo,
                                    LegalAddress = ac.BusinessUnit.LegalPerson.LegalAddress
                                },
                            Bank =
                                new LegalPersonBankInfo
                                {
                                    Name = ac.AccountSettings.LegalPersonBank.Bank.Name,
                                    Bik = ac.AccountSettings.LegalPersonBank.Bank.Bik,
                                    CorrespondentAccount = ac.AccountSettings.LegalPersonBank.Bank.CorrespondentAccount,
                                    SettlementAccount = ac.LegalPerson
                                        .LegalPersonBanks
                                        .Single(lpb => lpb.BankId == ac.AccountSettings.LegalPersonBank.Bank.Id)
                                        .SettlementAccount
                                },
                            Sum = ac.Sum,
                            Debt = ac.Debt,
                            TypeId = ac.TypeId
                        })
                .SingleAsync();

            var accountPositions = await _context.AccountPositions
                .Where(ap => ap.AccountId == accountId)
                .Select(ap =>
                    new AccountPositionInfo
                    {
                        Id = ap.Id,
                        Name = ap.Name,
                        Count = ap.Count,
                        Cost = ap.Cost,
                        Sum = ap.Sum
                    }
                )
                .ToListAsync();

            account.AccountPositions = accountPositions;

            var orderPositions = _context.OrderPositions
                .Include(op => op.Order.AccountOrder)
                .Include(op => op.Supplier).ThenInclude(su => su.Company)
                .Include(op => op.Supplier).ThenInclude(su => su.City)
                .Include(op => op.Price)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Supplier).ThenInclude(csu => csu.Company)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Supplier).ThenInclude(csu => csu.City)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Price)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PricePosition).ThenInclude(cpp => cpp.PricePositionType)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.GraphicPositions).ThenInclude(cgp => cgp.Graphic)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.RubricPositions).ThenInclude(crp => crp.Rubric)
                .Where(
                    op =>
                        op.ParentOrderPositionId == null &&
                        op.Order.AccountOrder.AccountId == accountId)
                .SelectPositions();

            account.OrderPositions = orderPositions;

            return account;
        }

        public async Task<ActionResult<Account>> GetFullAccountByIdAsync(int accountId)
        {
            var account = await _context.Accounts
                .Include(ac => ac.AccountSettings).ThenInclude(acs => acs.LegalPersonBank).ThenInclude(lpb => lpb.Bank)
                .Include(ac => ac.AccountOrders).ThenInclude(ao => ao.Order).ThenInclude(o => o.Manager)
                .Include(ac => ac.BusinessUnit).ThenInclude(bu => bu.LegalPerson).ThenInclude(lp => lp.LegalPersonSigns).ThenInclude(lps => lps.SignImages)
                .Include(ac => ac.BusinessUnit).ThenInclude(bu => bu.LegalPerson).ThenInclude(lp => lp.AccountSettings)
                .Include(ac => ac.BusinessUnit).ThenInclude(bu => bu.Cash).ThenInclude(c => c.LegalPersonBank).ThenInclude(lpb => lpb.Bank)
                .Include(ac => ac.LegalPerson).ThenInclude(lp => lp.LegalPersonBanks).ThenInclude(lpb => lpb.Bank)
                .Include(ac => ac.AccountPositions)
                .Where(ac => ac.Id == accountId)
                .SingleOrDefaultAsync();

            return account;
        }

        public async Task<ActionResult<IEnumerable<AccountInfoLight>>> GetAccountsAsync(int clientLegalPersonId, int startAccountId, int quantity)
        {
            var accounts = await _context.Accounts
                .Include(ac => ac.AccountOrders).ThenInclude(ao => ao.Order)
                .Include(ac => ac.BusinessUnit.LegalPerson)
                .Where(
                    ac =>
                        ac.LegalPersonId == clientLegalPersonId &&
                        ((startAccountId == 0) || ac.Id < startAccountId) &&
                        _businessUnitIds.Contains(ac.BusinessUnitId))
                .OrderByDescending(ac => ac.Id)
                .Take(quantity)
                .Select(
                    ac =>
                        new AccountInfoLight
                        {
                            Id = ac.Id,
                            Number = ac.Number,
                            Date = ac.Date,
                            SupplierLegalPerson =
                                new LegalPersonInfo
                                {
                                    Id = ac.BusinessUnit.LegalPersonId,
                                    Name = ac.BusinessUnit.LegalPerson.Name
                                },
                            Nds = ac.Nds,
                            Sum = ac.Sum,
                            Debt = ac.TypeId == 2
                                ? 0
                                : ac.Sum - ac.AccountOrders.Sum(ao => ao.Order.Paid),
                            TypeId = ac.TypeId
                        })
                .ToListAsync();

            return accounts;
        }

        public int GetAccountBusinessUnitIdAsync(int accountId)
        {
            var businessUnitId = _context.Accounts
                .Single(ac => ac.Id == accountId)
                .BusinessUnitId;

            return businessUnitId;
        }

        #endregion

        #region Create

        public int CreateClientAccount(int[] orderPositionIds)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                // Создаём заказ
                var orderPositions = _orderPositionProcessor.GetInnerOperationsOrderPositionsByIds(orderPositionIds);
                var basketOrder = _orderProcessor.GetOrderById(orderPositions.First().OrderId);
                var clientOrder = CreateClientOrder(basketOrder, orderPositions, dbTran);

                // Создаём счёт
                var account = _clientAccountProcessor.CreateClientAccount(
                    clientOrder.SupplierLegalPersonId, clientOrder.Sum, basketOrder, dbTran);

                // Создаём позиции счёта
                foreach (var orderPosition in orderPositions)
                {
                    // Позиция счёта создаётся только на позицию заказа, не входящую в пакет
                    if (orderPosition.ParentOrderPositionId == null)
                    {
                        var packagePositions = orderPosition.ChildOrderPositions.ToList();
                        _clientAccountProcessor.CreateAccountPosition(account.Id, orderPosition, packagePositions, dbTran);
                    }
                }

                // Создаём настройки счёта
                _accountSettingsProcessor.CreateAccountSettings(account.Id, basketOrder.ClientLegalPerson.AccountSettings, dbTran);

                // Создаём связку счёт-заказ
                _clientAccountProcessor.CreateAccountOrder(account.Id, clientOrder.Id, dbTran);

                _context.SaveChanges();
                dbTran.Commit();

                return account.Id;
            }
        }

        private Order CreateClientOrder(Order basketOrder, IEnumerable<OrderPosition> orderPositions, DbTransaction dbTran)
        {
            // Создаём клиентский заказ
            var order = _orderProcessor.CreateClientOrder(basketOrder, orderPositions, dbTran);

            // Привязываем к новому заказу позиции из корзины
            var orderPositionsWithChildren = orderPositions
                .Union(
                    orderPositions.SelectMany(op => op.ChildOrderPositions));

            foreach (var orderPosition in orderPositionsWithChildren)
                _orderPositionProcessor.UpdateOrderPosition(orderPosition, order.Id, dbTran);

            // Привязываем к новому заказу позиции ИМ-ов
            var positionIms = orderPositionsWithChildren
                .Where(op => op.PositionIm != null)
                .Select(op => op.PositionIm);

            foreach (var positionIm in positionIms)
                // Меняем заказ и статус позиции ИМ-а на "Вёрстка"
                _positionImProcessor.UpdatePositionIm(positionIm, order.Id, maketStatusId: 3, dbTran);

            var orderImTypeIds = positionIms
                .GroupBy(pims => pims.PositionImType.OrderImType.Id)
                .Select(g => g.Key)
                .ToList();

            foreach (var orderImTypeId in orderImTypeIds)
            {
                // Создаём ИМ нового заказа
                var orderIm = _orderImProcessor.CreateOrderIm(order.Id, orderImTypeId,
                    _orderImProcessor.ProcessOrderImStatus, dbTran);

                // Если ИМ заказа корзины больше не нужен - удаляем его
                var basketOrderIm = _orderImProcessor.GetOrderIm(basketOrder.Id, orderImTypeId);
                if (_orderImProcessor.NeedDeleteOrderIm(basketOrderIm))
                    _orderImProcessor.DeleteOrderIm(basketOrderIm, dbTran);
            }

            // Обновляем заказ-корзину
            _orderProcessor.UpdateOrder(basketOrder.Id, dbTran);

            return order;
        }

        #endregion
    }
}
