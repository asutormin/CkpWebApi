using CkpDAL.Entities;
using CkpModel.Input;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IOrderPositionFactory
    {
        OrderPosition Create(int orderId, int? parentOrderPositionId, float discount, float markup, float nds, OrderPositionData opd);
    }
}
