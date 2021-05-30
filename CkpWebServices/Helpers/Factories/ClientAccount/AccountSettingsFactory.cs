using CkpDAL.Model;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using System;

namespace CkpServices.Helpers.Factories.ClientAccount
{
    class AccountSettingsFactory : IAccountSettingsFactory
    {
        public AccountSettings Create(int accountId, AccountSettings legalPersonAccountSettings)
        {
            var accountSettings = new AccountSettings
            {
                Id = 0,
                AccountId = accountId,
                LegalPersonId = null,
                LegalPersonBankId = legalPersonAccountSettings.LegalPersonBankId,
                UnloadingDateMethod = legalPersonAccountSettings.UnloadingDateMethod,
                UnloadingTypeId = legalPersonAccountSettings.UnloadingTypeId,
                UnloadingDayNumber = legalPersonAccountSettings.UnloadingDayNumber,
                AdditionalDescription = legalPersonAccountSettings.AdditionalDescription,
                AccountDescription = legalPersonAccountSettings.AccountDescription,
                ActDescription = legalPersonAccountSettings.ActDescription,
                ShortAccountPositionFormulation = legalPersonAccountSettings.ShortAccountPositionFormulation,
                AdvanceAccountPositionFormulation = legalPersonAccountSettings.AdvanceAccountPositionFormulation,
                ContractNumber = legalPersonAccountSettings.ContractNumber,
                ContractDate = legalPersonAccountSettings.ContractDate,
                ShowAdditionalDescription = legalPersonAccountSettings.ShowAdditionalDescription,
                ShowDetailed = legalPersonAccountSettings.ShowDetailed,
                ShowContractInCaption = legalPersonAccountSettings.ShowContractInCaption,
                ShowOkpo = legalPersonAccountSettings.ShowOkpo,
                ShowSupplier = legalPersonAccountSettings.ShowSupplier,
                ShowPositionName = legalPersonAccountSettings.ShowPositionName,
                ShowExitDate = legalPersonAccountSettings.ShowExitDate,
                ShowExitNumber = legalPersonAccountSettings.ShowExitNumber,
                ShowContract = legalPersonAccountSettings.ShowContract,
                ShowDiscount = legalPersonAccountSettings.ShowDiscount,
                BeginDate = DateTime.Now
            };

            return accountSettings;
        }
    }
}
