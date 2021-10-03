using CkpDAL.Entities;
using CkpModel.Input;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderPositionProcessor
    {
        IEnumerable<OrderPosition> GetInnerOperationsOrderPositionsByIds(int[] orderPositionIds);
        IQueryable<OrderPosition> GetBasketOrderPositionsQuery(int clientLegalPersonId);
        IQueryable<OrderPosition> GetAccountOrderPositionsQuery(int clientLegalPersonId, int accountId);
        IQueryable<OrderPosition> GetOrderPositionsByIdsOuery(int clientLegalPersonId, int[] orderPositionIds);
        bool NeedCreateFullOrderPosition(OrderPosition orderPosition);
        int CreateFullOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, OrderPositionData opd, DbTransaction dbTran);
        void UpdateFullOrderPosition(OrderPosition orderPosition, OrderPositionData opd, DbTransaction dbTran);
        void UpdateOrderPosition(OrderPosition orderPosition, int orderId, DbTransaction dbTran);
        bool NeedDeleteFullOrderPosition(int orderPositionId, List<OrderPositionData> opds);
        void DeleteFullOrderPosition(OrderPosition orderPosition, DbTransaction dbTran);
    }
}
