using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces.ClientAccount
{
    interface IAccountPositionFactory
    {
        AccountPosition Create(int accountId, OrderPosition orderPosition);
    }
}
