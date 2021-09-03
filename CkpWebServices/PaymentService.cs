using CkpDAL;
using CkpDAL.Repository;
using CkpInfrastructure.Configuration;
using CkpModel.Output;
using CkpServices.Helpers.Providers;
using CkpServices.Interfaces;
using CkpServices.Processors;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices
{
    public class PaymentService : IPaymentService
    {
        private readonly BPFinanceContext _context;
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            var orderSettings = appParamsAccessor.Value.OrderSettings;
            var orderBusinessUnitIdsProvider = new OrderBusinessUnitIdsProvider(orderSettings);

            _paymentProcessor = new PaymentProcessor(
                _context,
                repository,
                orderBusinessUnitIdsProvider);
        }

        public List<BalanceInfo> GetBalance(int clientLegalPersonId)
        {
            var undisposedPayments = _paymentProcessor
                .GetUndisposedPaymentsByLegalPersonId(clientLegalPersonId);

            var pg = undisposedPayments
                .GroupBy(p => p.BusinessUnit.LegalPerson)
                .Select(g => new { LegalPersonName = g.Key.Name, Sum = g.Sum(g => g.UndisposedSum) });

            var unpaidOrders = _paymentProcessor
                .GetUnpaidAdvancedOrdersByLegalPersonId(clientLegalPersonId);

            var og = unpaidOrders
                .GroupBy(o => o.BusinessUnit.LegalPerson)
                .Select(g => new { LegalPersonName = g.Key.Name, Sum = g.Sum(g => g.Paid - g.Sum) });

            var balance = pg.Union(og)
                .GroupBy(g => g.LegalPersonName)
                .Select(
                    g => 
                        new BalanceInfo
                        {
                            SupplierLegalPersonName = g.Key,
                            BalanceSum = g.Sum(g => g.Sum) 
                        })
                .ToList();

            return balance;
        }

        public void PayAdvanceOrders(int clientLegalPersonId)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var advanceOrders = _paymentProcessor
                    .GetAdvancedOrdersByLegalPersonId(clientLegalPersonId)
                    .OrderBy(o => o.ClientLegalPersonId).ThenBy(o => o.Id);

                // Перебираем не оплаченные заказы с оплатой из аванса
                foreach (var order in advanceOrders)
                {
                    var undisposedPayments = _paymentProcessor
                        .GetUndisposedPaymentsByOrder(order)
                        .OrderBy(p => p.PaymentDate);

                    var orderDebt = order.Sum - order.Paid;

                    // Перебираем платежи и распределяем их на заказ
                    foreach (var payment in undisposedPayments)
                    {
                        // Если заказ оплачен - выходим из цикла
                        if (orderDebt == 0)
                            break;

                        // Если остаток платежа меньше не распределённой части заказа
                        // полностью распределяем его на заказ
                        // Иначе распределяем на заказ только часть суммы,
                        // необходимую для оплаты
                        var orderPaymentSum = payment.UndisposedSum <= orderDebt
                            ? payment.UndisposedSum
                            : orderDebt;

                        var undisposedSum = payment.UndisposedSum - orderPaymentSum;
                        _paymentProcessor.UpdateUndisposedSum(payment, undisposedSum, dbTran);

                        _paymentProcessor.CreateOrderPayment(order, payment, orderPaymentSum, dbTran);

                        orderDebt -= orderPaymentSum;
                    }
                }

                _context.SaveChanges();
                dbTran.Commit();
            }
        }
    }
}
