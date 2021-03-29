using CkpDAL.Model;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using System;

namespace CkpServices.Helpers.Factories.ClientAccount
{
    class AccountOrderFactory : IAccountOrderFactory
    {
        public AccountOrder Create(int accountId, int orderId)
        {
            var accountOrder = new AccountOrder
            {
                Id = 0,
                AccountId = accountId,
                OrderId = orderId,
                BeginDate = DateTime.Now
            };

            return accountOrder;
        }
    }
}
