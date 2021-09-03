using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IOrderPaymentFactory
    {
        OrderPayment Create(int orderId, int paymentId, float sum);
    }
}
