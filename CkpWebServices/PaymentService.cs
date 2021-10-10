using CkpDAL;
using CkpDAL.Repository;
using CkpInfrastructure.Configuration;
using CkpModel.Output;
using CkpServices.Interfaces;
using CkpServices.Processors;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices
{
    public class PaymentService : IPaymentService
    {
        private readonly BPFinanceContext _context;
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            _paymentProcessor = new PaymentProcessor(
                _context,
                repository,
                appParamsAccessor.Value.BusinessUnitIds);
        }

        public List<BalanceInfo> GetBalances(int clientLegalPersonId)
        {
            var legalPersonsZeroBalance = _paymentProcessor
                .GetLegalPersonsZeroBalance()
                .Select(b => new
                {
                    BusinessUnitId = b.Item1,
                    LegalPersonName = b.Item2,
                    Sum = b.Item3
                });

            var undisposedPayments = _paymentProcessor
                .GetUndisposedPaymentsByLegalPersonId(clientLegalPersonId);

            var pg = undisposedPayments
                .GroupBy(p => p.BusinessUnit)
                .Select(g => new 
                {
                    BusinessUnitId = g.Key.Id,
                    LegalPersonName = g.Key.LegalPerson.Name,
                    Sum = g.Sum(g => g.UndisposedSum)
                })
                .Union(legalPersonsZeroBalance);

            var unpaidOrders = _paymentProcessor
                .GetUnpaidOrdersByLegalPersonId(clientLegalPersonId);

            var og = unpaidOrders
                .GroupBy(o => o.BusinessUnit)
                .Select(g => new 
                {
                    BusinessUnitId = g.Key.Id,
                    LegalPersonName = g.Key.LegalPerson.Name,
                    Sum = g.Sum(g => g.Paid - g.Sum)
                })
                .Union(legalPersonsZeroBalance);

            var balances = pg.Union(og)
                .GroupBy(g => g.BusinessUnitId)
                .Select(
                    g => 
                        new BalanceInfo
                        {
                            BusinessUnitId = g.Key,
                            SupplierLegalPersonName = g.First().LegalPersonName,
                            BalanceSum = g.Sum(g => g.Sum) 
                        })
                .ToList();

            return balances;
        }

        public void PayOrders(int clientLegalPersonId)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var unpaidOrders = _paymentProcessor
                    .GetUnpaidOrdersByLegalPersonId(clientLegalPersonId)
                    .OrderBy(o => o.ClientLegalPersonId).ThenBy(o => o.Id);

                // Перебираем не оплаченные заказы с оплатой из аванса
                foreach (var order in unpaidOrders)
                {
                    var undisposedPayments = _paymentProcessor
                        .GetUndisposedPaymentsByOrder(order)
                        .OrderBy(p => p.PaymentDate);

                    var orderDebt = order.Sum - order.Paid;

                    // Перебираем платежи и распределяем их на заказ
                    foreach (var payment in undisposedPayments)
                    {
                        // Если заказ оплачен - выходим из цикла
                        if (orderDebt == 0)
                            break;

                        // Если остаток платежа меньше не распределённой части заказа
                        // полностью распределяем его на заказ
                        // Иначе распределяем на заказ только часть суммы,
                        // необходимую для оплаты
                        var orderPaymentSum = payment.UndisposedSum <= orderDebt
                            ? payment.UndisposedSum
                            : orderDebt;

                        var undisposedSum = payment.UndisposedSum - orderPaymentSum;
                        _paymentProcessor.AddOrderToPayment(payment, order);
                        _paymentProcessor.UpdateUndisposedSum(payment, undisposedSum, dbTran);

                        _paymentProcessor.CreateOrderPayment(order, payment, orderPaymentSum, dbTran);
                        _paymentProcessor.UpdateOrderPaidSum(order, dbTran);

                        orderDebt -= orderPaymentSum;
                    }
                }

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        public bool CanApplyInTimeDiscount(int clientLegalPersonId, int accountId)
        {
            var balances = GetBalances(clientLegalPersonId);

            var account = _context.Accounts.Single(ac => ac.Id == accountId);

            var balance = balances
                .SingleOrDefault(b => b.BusinessUnitId == account.BusinessUnitId);

            return balance.BalanceSum >= account.Sum;
        }
    }
}
