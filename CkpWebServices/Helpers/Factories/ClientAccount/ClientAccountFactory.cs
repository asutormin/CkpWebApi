using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using System;

namespace CkpServices.Helpers.Factories.ClientAccount
{
    class ClientAccountFactory : IClientAccountFactory
    {
        public Account Create(string number, float sum, BusinessUnit businessUnit, LegalPerson clientLegalPerson)
        {
            var account = new Account
            {
                Id = 0,
                Number = number,
                Date = DateTime.Now.Date,
                CompanyId = clientLegalPerson.CompanyId,
                LegalPersonId = clientLegalPerson.Id,
                CashId = businessUnit.CashId,
                BusinessUnitId = businessUnit.Id,
                StatusId = 3,
                TypeId = clientLegalPerson.AccountSettings.IsNeedPrepayment ? 3 : 1,
                Sum = sum,
                Nds = businessUnit.AccountsWithNds ? 20 : 0,
                Description = null,
                AdditionalDescription = null,
                Request = null,
                AccountSettings = null,
                Printed = false,
                UnloadedTo1C = false,
                Prepaid = 0,
                Debt = 0,
                PaymentAgentId = 1, // Отсутствует
                PaymentAgentCommissionSum = 0,
                BeginDate = DateTime.Now
            };

            return account;
        }
    }
}
