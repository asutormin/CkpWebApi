using CkpWebApi.OutputEntities;
using CkpWebApi.Helpers;
using DebtsWebApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CkpWebApi.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace CkpWebApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly int _orderBusinessUnitId;

        private readonly BPFinanceContext _context;

        public AccountService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;

            _orderBusinessUnitId = appParamsAccessor.Value.OrderBusinessUnitId;
        }

        public async Task<ActionResult<AccountInfo>> GetAccountAsync(int accountId)
        {
            var account = await _context.Accounts
                .Include(ac => ac.ClientLegalPerson)
                .Include(ac => ac.BusinessUnit.LegalPerson)
                .Include(ac => ac.AccountSettings.Bank)
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
                                    Id = ac.ClientLegalPersonId,
                                    Name = ac.ClientLegalPerson.Name,
                                    Inn = ac.ClientLegalPerson.Inn,
                                    Kpp = ac.ClientLegalPerson.Kpp,
                                    Okpo = ac.ClientLegalPerson.Okpo,
                                    LegalAddress = ac.ClientLegalPerson.LegalAddress
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
                                    Name = ac.AccountSettings.Bank.Name,
                                    Bik = ac.AccountSettings.Bank.Bik,
                                    CorrespondentAccount = ac.AccountSettings.Bank.CorrespondentAccount,
                                    SettlementAccount = ac.ClientLegalPerson
                                        .LegalPersonBanks
                                        .Single(lpb => lpb.BankId == ac.AccountSettings.Bank.Id)
                                        .SettlementAccount
                                },
                            Sum = ac.Sum,
                            Debt = ac.Debt
                        })
                .SingleAsync();

            var positions = await _context.OrderPositions
                .Include(op => op.Order.AccountOrder)
                .Include(op => op.ParentOrderPosition)
                .Include(op => op.Price)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Where(
                    op =>
                        op.Order.AccountOrder.AccountId == accountId)
                .SelectPositions()
                .ToListAsync();

            account.Positions = positions;

            return account;
        }

        public async Task<ActionResult<IEnumerable<AccountLight>>> GetAccountsAsync(int clientLegalPersonId, int startAccountId, int quantity)
        {
            var accounts = await _context.Accounts
                .Include(ac => ac.AccountOrders).ThenInclude(ao => ao.Order)
                .Include(ac => ac.BusinessUnit.LegalPerson)
                .Where(
                    ac =>
                        ac.ClientLegalPersonId == clientLegalPersonId &&
                        ((startAccountId == 0) || ac.Id < startAccountId) &&
                        ac.BusinessUnitId == _orderBusinessUnitId)
                .OrderByDescending(ac => ac.Id)
                .Take(quantity)
                .Select(
                    ac =>
                        new AccountLight
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
                            Debt = ac.Sum - ac.AccountOrders.Sum(ao => ao.Order.Paid)
                        })
                .ToListAsync();

            return accounts;
        }
    }
}
