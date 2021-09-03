using CkpDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CkpServices.Processors.Interfaces
{
    interface IPaymentProcessor
    {
        List<Order> GetAdvancedOrdersByLegalPersonId(int clientLegalPersonId);

        List<Order> GetUnpaidAdvancedOrdersByLegalPersonId(int clientLegalPersonId);

        List<Payment> GetUndisposedPaymentsByLegalPersonId(int clientLegalPersonId);

        List<Payment> GetUndisposedPaymentsByOrder(Order order);

        OrderPayment CreateOrderPayment(
            Order order, Payment payment, float sum, DbTransaction dbTran);

        void UpdateUndisposedSum(Payment payment, float sum, DbTransaction dbTran);

        void UpdateOrderPaidSum(Order order, DbTransaction dbTran);
    }
}
