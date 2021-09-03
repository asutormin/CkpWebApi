using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class OrderPaymentFactory : IOrderPaymentFactory
    {
        public OrderPayment Create(int orderId, int paymentId, float sum)
        {
            var orderPayment = new OrderPayment
            {
                OrderId = orderId,
                PaymentId = paymentId,
                PaidSum = sum,
                DistributionDate = DateTime.Now,
                Description = "Форма ЦКП"
            };

            return orderPayment;
        }
    }
}
