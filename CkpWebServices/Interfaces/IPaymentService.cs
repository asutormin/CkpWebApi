using CkpModel.Output;
using System.Collections.Generic;

namespace CkpServices.Interfaces
{
    public interface IPaymentService
    {
        void PayAdvanceOrders(int clientLegalPersonId);
        List<BalanceInfo> GetBalance(int clientLegalPersonId);
    }
}
