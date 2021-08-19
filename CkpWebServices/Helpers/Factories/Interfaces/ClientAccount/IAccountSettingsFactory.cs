using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces.ClientAccount
{
    interface IAccountSettingsFactory
    {
        AccountSettings Create(int accountId, AccountSettings legalPersonAccountSettings);
    }
}
