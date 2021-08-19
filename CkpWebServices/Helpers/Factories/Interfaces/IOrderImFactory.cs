using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IOrderImFactory
    {
        OrderIm Create(int orderId, int orderImTypeId);
    }
}
