using CkpDAL.Model;
using CkpServices.Helpers.Factories.Interfaces;
using System;
using System.Collections.Generic;

namespace CkpServices.Helpers.Factories
{
    class ClientOrderFactory : IClientOrderFactory
    {
        private readonly int _editUserId;

        public ClientOrderFactory(int editUserId)
        {
            _editUserId = editUserId;
        }

        public Order Create(Order shoppingCartOrder, IEnumerable<OrderPosition> orderPositions)
        {
            var sum = orderPositions.GetClientSum();
            var maxExitDate = orderPositions.GetMaxExitDate();

            var order = new Order
            {
                Id = 0,
                ParentOrderId = null,
                ActivityTypeId = 1,
                BusinessUnitId = shoppingCartOrder.BusinessUnitId,
                StatusId = 0, // ---
                IsNeedVisa = false,
                IsNeedAccount = true,
                AccountDescription = string.Empty,
                OrderNumber = string.Empty,
                OrderDate = DateTime.Now.Date,
                MaxExitDate = maxExitDate,
                ClientCompanyId = shoppingCartOrder.ClientCompanyId,
                ClientLegalPersonId = shoppingCartOrder.ClientLegalPersonId,
                SupplierLegalPersonId = shoppingCartOrder.SupplierLegalPersonId,
                Sum = sum,
                Paid = 0,
                IsCashless = false,
                IsAdvance = false,
                IsPaymentWithAgent = false,
                IsFactoring = false,
                CreatedPaymentPrognosisTypeId = 1,
                CurrentPaymentPrognosisTypeId = 1,
                PaymentArbitaryPrognosisDate = null,
                Description = string.Empty,
                Request = string.Empty,
                ManagerId = shoppingCartOrder.ManagerId,
                BeginDate = DateTime.Now,
                EditUserId = _editUserId
            };

            return order;
        }
    }
}
