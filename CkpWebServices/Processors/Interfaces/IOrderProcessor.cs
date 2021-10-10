using CkpDAL.Entities;
using CkpModel.Input;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderProcessor
    {
        Order GetOrderById(int orderId);
        Order GetBasketOrder(int clientLegalPersonId, int supplierId);
        Order CreateBasketOrder(OrderPositionData opd, DbTransaction dbTran);
        Order CreateClientOrder(Order basketOrder, IEnumerable<OrderPosition> orderPositions, DbTransaction dbTran);
        void UpdateOrder(int orderId, DbTransaction dbTran);
        void UpdateOrder(Order order, DbTransaction dbTran);
        void RefreshOrder(Order order, DbTransaction dbTran);
    }
}
