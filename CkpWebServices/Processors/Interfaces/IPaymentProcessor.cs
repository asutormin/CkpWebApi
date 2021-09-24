using CkpDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IPaymentProcessor
    {
        List<Order> GetUnpaidOrdersByLegalPersonId(int clientLegalPersonId);
        List<Payment> GetUndisposedPaymentsByLegalPersonId(int clientLegalPersonId);
        List<Payment> GetUndisposedPaymentsByOrder(Order order);
        List<Tuple<int, string, float>> GetLegalPersonsZeroBalance();
        OrderPayment CreateOrderPayment(
            Order order, Payment payment, float sum, DbTransaction dbTran);
        void UpdateUndisposedSum(Payment payment, float sum, DbTransaction dbTran);
        void AddOrderToPayment(Payment payment, Order order);
        void UpdateOrderPaidSum(Order order, DbTransaction dbTran);
    }
}
