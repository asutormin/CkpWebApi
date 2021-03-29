using CkpDAL.Model;
using CkpEntities.Input;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IOrderPositionFactory
    {
        OrderPosition Create(int orderId, int? parentOrderPositionId, float discount, float markup, float nds, Advertisement adv);
    }
}
