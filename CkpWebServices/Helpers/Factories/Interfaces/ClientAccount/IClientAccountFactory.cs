using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces.ClientAccount
{
    interface IClientAccountFactory
    {
        Account Create(string number, float sum, Order basketOrder);
    }
}
