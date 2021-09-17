using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces;
using System;
using System.Collections.Generic;

namespace CkpServices.Helpers.Factories
{
    class ClientOrderFactory : IClientOrderFactory
    {
        public Order Create(Order basketOrder, IEnumerable<OrderPosition> orderPositions)
        {
            var sum = orderPositions.GetClientSum();
            var maxExitDate = orderPositions.GetMaxExitDate();

            var order = new Order
            {
                Id = 0,
                ParentOrderId = null,
                ActivityTypeId = 1,
                BusinessUnitId = basketOrder.BusinessUnitId,
                StatusId = 0, // ---
                IsNeedVisa = false,
                IsNeedAccount = true,
                AccountDescription = string.Empty,
                OrderNumber = string.Empty,
                OrderDate = DateTime.Now.Date,
                MaxExitDate = maxExitDate,
                ClientCompanyId = basketOrder.ClientCompanyId,
                ClientLegalPersonId = basketOrder.ClientLegalPersonId,
                SupplierLegalPersonId = basketOrder.SupplierLegalPersonId,
                Sum = sum,
                Paid = 0,
                IsCashless = false,
                IsAdvance = basketOrder.IsAdvance,
                IsPaymentWithAgent = false,
                IsFactoring = false,
                CreatedPaymentPrognosisTypeId = 1,
                CurrentPaymentPrognosisTypeId = 1,
                PaymentArbitaryPrognosisDate = null,
                Description = string.Empty,
                Request = string.Empty,
                ManagerId = basketOrder.ManagerId,
                BeginDate = DateTime.Now
            };

            return order;
        }
    }
}
