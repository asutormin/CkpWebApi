using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors
{
    class PaymentProcessor : IPaymentProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private int[] _businessUnitIds;
        private readonly IOrderPaymentFactory _orderPaymentFactory;

        public PaymentProcessor(
            BPFinanceContext context,
            IBPFinanceRepository repository,
            int[] businessUnits)
        {
            _context = context;
            _repository = repository;

            _businessUnitIds = businessUnits;

            _orderPaymentFactory = new OrderPaymentFactory();
        }

        public List<Order> GetAdvanceOrdersByLegalPersonId(int clientLegalPersonId)
        {
            var advanceOrders = _context.Orders
                    .Include(o => o.AccountOrder).ThenInclude(ao => ao.Account)
                    .Where(
                        o =>
                            o.ClientLegalPerson.Id == clientLegalPersonId &&
                            o.ActivityTypeId == 1 && // КР заказ
                            o.IsAdvance &&
                            o.AccountOrder.Account.TypeId == 3 && // Выставлен фиктивный счёт
                            _businessUnitIds.Contains(o.BusinessUnitId) &&
                            o.Paid < o.Sum)
                    .ToList();

            return advanceOrders;
        }

        public List<Order> GetUnpaidOrdersByLegalPersonId(int clientLegalPersonId)
        {
            var unpaidOrders = _context.Orders
                .Include(o => o.BusinessUnit).ThenInclude(bu => bu.LegalPerson)
                .Include(o => o.AccountOrder).ThenInclude(ao => ao.Account)
                .Where(
                    o =>
                        o.ClientLegalPersonId == clientLegalPersonId &&
                        o.ActivityTypeId == 1 && // КР заказ
                        _businessUnitIds.Contains(o.BusinessUnitId) &&
                        o.Paid < o.Sum)
                .ToList();

            return unpaidOrders;
        }

        public List<Payment> GetUndisposedPaymentsByOrder(Order order)
        {
            var undisposedPayments = _context.Payments
                .Where(
                    p =>
                        p.PaymentTypeId == 2 && // От клиента
                        p.LegalPersonId == order.ClientLegalPersonId &&
                        p.BusinessUnitId == order.BusinessUnitId &&
                        p.CashId == order.AccountOrder.Account.CashId &&
                        p.UndisposedSum > 0)
                .ToList();

            return undisposedPayments;
        }

        public List<Payment> GetUndisposedPaymentsByLegalPersonId(int clientLegalPersonId)
        {
            var undisposedPayments = _context.Payments
                .Include(p => p.BusinessUnit).ThenInclude(bu => bu.LegalPerson)
                .Where(
                    p =>
                        p.PaymentTypeId == 2 && // От клиента
                        p.LegalPersonId == clientLegalPersonId &&
                        _businessUnitIds.Contains(p.BusinessUnitId) &&
                        p.UndisposedSum > 0)
                .ToList();

            return undisposedPayments;
        }

        public List<Tuple<int, string, float>> GetLegalPersonsZeroBalance()
        {
            var legalPersonsZeroBalance = _context.BusinessUnits
                .Include(bu => bu.LegalPerson)
                .Where(
                    bu => _businessUnitIds.Contains(bu.Id))
                .Select(
                    bu =>
                        new Tuple<int, string, float>(bu.Id, bu.LegalPerson.Name, 0))
                .ToList();

            return legalPersonsZeroBalance;
        }

        public OrderPayment CreateOrderPayment(
            Order order, Payment payment, float sum, DbTransaction dbTran)
        {
            var orderPayment = _orderPaymentFactory.Create(
                order.Id, payment.Id, sum);

            _repository.SetOrderPayment(orderPayment, true, dbTran);

            return orderPayment;
        }

        public void UpdateUndisposedSum(Payment payment, float sum, DbTransaction dbTran)
        {
            payment.UndisposedSum = sum;

            _repository.SetPayment(payment, true, dbTran);
        }

        public void AddOrderToPayment(Payment payment, Order order)
        {
            var orderNumber = order.Id.ToString();

            payment.OrdersNumber = string.IsNullOrEmpty(payment.OrdersNumber)
                ? orderNumber
                : string.Format("{0}, {1}", payment.OrdersNumber, orderNumber);
        }

        public void UpdateOrderPaidSum(Order order, DbTransaction dbTran)
        {
            _repository.ChangeOrderPaid(order, dbTran);
        }


    }
}
