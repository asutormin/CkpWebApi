using CkpDAL.Model;
using CkpEntities.Input;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderProcessor
    {
        Order GetOrderById(int orderId);
        Order GetShoppingCartOrder(int clientLegalPersonId);
        IQueryable<OrderPosition> GetShoppingCartOrderPositionsQuery(int clientLegalPersonId);
        Order CreateShoppingCartOrder(Advertisement adv, DbTransaction dbTran);
        Order CreateClientOrder(Order shoppingCartOrder, IEnumerable<OrderPosition> orderPositions, DbTransaction dbTran);
        void UpdateOrder(int orderId, DbTransaction dbTran);
        void RefreshOrder(Order order, DbTransaction dbTran);
    }
}
