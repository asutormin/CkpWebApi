using CkpDAL.Entities;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    public interface IAccountSettingsProcessor
    {
        AccountSettings GetAccountSettingsByLegalPersonId(int clientLegalPersonId);
        AccountSettings CreateAccountSettings(int accountId, AccountSettings legalPersonAccountSettings, DbTransaction dbTran);
        void UpdateAccountSettings(AccountSettings accountSettings, DbTransaction dbTran);
    }
}
