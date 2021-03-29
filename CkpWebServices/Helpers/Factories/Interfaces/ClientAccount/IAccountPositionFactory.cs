using CkpDAL.Model;

namespace CkpServices.Helpers.Factories.Interfaces.ClientAccount
{
    interface IAccountPositionFactory
    {
        AccountPosition Create(int accountId, OrderPosition orderPosition);
    }
}
