﻿using CkpWebApi.OutputEntities;
using CkpWebApi.Helpers;
using DebtsWebApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CkpWebApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using CkpWebApi.DAL.Model;

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
                                    Name = ac.AccountSettings.LegalPersonBank.Bank.Name,
                                    Bik = ac.AccountSettings.LegalPersonBank.Bank.Bik,
                                    CorrespondentAccount = ac.AccountSettings.LegalPersonBank.Bank.CorrespondentAccount,
                                    SettlementAccount = ac.ClientLegalPerson
                                        .LegalPersonBanks
                                        .Single(lpb => lpb.BankId == ac.AccountSettings.LegalPersonBank.Bank.Id)
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
                        op.ParentOrderPositionId == null &&
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

        public async Task<ActionResult<Account>> GetAccountById(int accountId)
        {
            var account = await _context.Accounts
                .Include(ac => ac.AccountSettings).ThenInclude(acs => acs.LegalPersonBank).ThenInclude(lpb => lpb.Bank)
                .Include(ac => ac.AccountOrders).ThenInclude(ao => ao.Order).ThenInclude(o => o.Manager)
                .Include(ac => ac.BusinessUnit).ThenInclude(bu => bu.LegalPerson).ThenInclude(lp => lp.LegalPersonSigns).ThenInclude(lps => lps.SignImages)
                .Include(ac => ac.BusinessUnit).ThenInclude(bu => bu.LegalPerson).ThenInclude(lp => lp.AccountSettings)
                .Include(ac => ac.BusinessUnit).ThenInclude(bu => bu.Cash).ThenInclude(c => c.LegalPersonBank).ThenInclude(lpb => lpb.Bank)
                .Include(ac => ac.ClientLegalPerson).ThenInclude(lp => lp.LegalPersonBanks).ThenInclude(lpb => lpb.Bank)
                .Include(ac => ac.AccountPositions)
                .Where(ac => ac.Id == accountId)
                .SingleOrDefaultAsync();

            return account;
        }
    }
}
