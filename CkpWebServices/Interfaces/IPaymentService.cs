using CkpModel.Output;
using System.Collections.Generic;

namespace CkpServices.Interfaces
{
    public interface IPaymentService
    {
        void PayOrders(int clientLegalPersonId);
        List<BalanceInfo> GetBalance(int clientLegalPersonId);
    }
}
