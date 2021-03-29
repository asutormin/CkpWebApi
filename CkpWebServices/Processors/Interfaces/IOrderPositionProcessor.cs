using CkpDAL.Model;
using CkpEntities.Input;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderPositionProcessor
    {
        IEnumerable<OrderPosition> GetOrderPositionsByIds(int[] orderPositionIds);
        bool NeedCreateFullOrderPosition(OrderPosition orderPosition);
        int CreateFullOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, Advertisement adv, DbTransaction dbTran);
        void UpdateFullOrderPosition(OrderPosition orderPosition, Advertisement adv, DbTransaction dbTran);
        void UpdateOrderPosition(OrderPosition orderPosition, int orderId, DbTransaction dbTran);
        bool NeedDeleteFullOrderPosition(int orderPositionId, List<Advertisement> advs);
        void DeleteFullOrderPosition(OrderPosition orderPosition, DbTransaction dbTran);
    }
}
