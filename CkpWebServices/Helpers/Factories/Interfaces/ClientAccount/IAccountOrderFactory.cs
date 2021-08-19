using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces.ClientAccount
{
    interface IAccountOrderFactory
    {
        AccountOrder Create(int accountId, int orderId);
    }
}
