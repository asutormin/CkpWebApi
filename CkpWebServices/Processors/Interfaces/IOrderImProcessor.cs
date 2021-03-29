using CkpDAL.Model;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IOrderImProcessor
    {
        OrderIm GetOrderIm(int orderId, int orderImTypeId);
        OrderIm CreateOrderIm(int orderId, int orderImTypeId, DbTransaction dbTran);
        OrderIm UpdateOrderIm(OrderIm orderIm, DbTransaction dbTran);
        bool NeedDeleteOrderIm(OrderIm orderIm);
        void DeleteOrderIm(OrderIm orderIm, DbTransaction dbTran);
        void ProcessOrderImStatus(OrderIm orderIm, DbTransaction dbTran);
    }
}
