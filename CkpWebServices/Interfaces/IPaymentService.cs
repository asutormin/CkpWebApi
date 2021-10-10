using CkpModel.Output;
using System.Collections.Generic;

namespace CkpServices.Interfaces
{
    public interface IPaymentService
    {
        void PayOrders(int clientLegalPersonId);
        List<BalanceInfo> GetBalances(int clientLegalPersonId);
        bool CanApplyInTimeDiscount(int clientLegalPersonId, int accountId);
    }
}
