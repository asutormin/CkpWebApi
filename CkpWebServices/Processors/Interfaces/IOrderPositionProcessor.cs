using CkpDAL.Entities;
using CkpModel.Input;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderPositionProcessor
    {
        IEnumerable<OrderPosition> GetOrderPositionsByIds(int[] orderPositionIds);
        bool NeedCreateFullOrderPosition(OrderPosition orderPosition);
        int CreateFullOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, OrderPositionData opd, DbTransaction dbTran);
        void UpdateFullOrderPosition(OrderPosition orderPosition, OrderPositionData opd, DbTransaction dbTran);
        void UpdateOrderPosition(OrderPosition orderPosition, int orderId, DbTransaction dbTran);
        bool NeedDeleteFullOrderPosition(int orderPositionId, List<OrderPositionData> opds);
        void DeleteFullOrderPosition(OrderPosition orderPosition, DbTransaction dbTran);
    }
}
