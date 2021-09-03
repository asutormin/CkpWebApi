using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class BasketOrderFactory : IBasketOrderFactory
    {
        private readonly string _orderDescription;

        public BasketOrderFactory(string orderDescription)
        {
            _orderDescription = orderDescription;
        }

        public Order Create(
            int businessUnitId,
            int clientLegalPersonId,
            int clientCompanyId,
            int supplierLegalPersonId,
            DateTime maxExitDate,
            float sum,
            int managerId)
        {
            var order = new Order
            {
                Id = 0,
                ParentOrderId = null,
                ActivityTypeId = 20,
                BusinessUnitId = businessUnitId,
                StatusId = 1, // Редактирование
                IsNeedVisa = false,
                IsNeedAccount = false,
                AccountDescription = string.Empty,
                OrderNumber = string.Empty,
                OrderDate = DateTime.Now.Date,
                MaxExitDate = maxExitDate,
                ClientCompanyId = clientCompanyId,
                ClientLegalPersonId = clientLegalPersonId,
                SupplierLegalPersonId = supplierLegalPersonId,
                Sum = sum,
                Paid = 0,
                IsCashless = false,
                IsAdvance = false,
                IsPaymentWithAgent = false,
                IsFactoring = false,
                CreatedPaymentPrognosisTypeId = 1,
                CurrentPaymentPrognosisTypeId = 1,
                PaymentArbitaryPrognosisDate = null,
                Description = _orderDescription,
                Request = string.Empty,
                ManagerId = managerId,
                BeginDate = DateTime.Now
            };

            return order;
        }
    }
}
