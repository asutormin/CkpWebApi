using CkpDAL.Entities;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IClientAccountProcessor
    {
        Account CreateClientAccount(int supplierLegalPersonId, float sum, Order basketOrder, DbTransaction dbTran);
        AccountPosition CreateAccountPosition(int accountId, OrderPosition orderPosition, List<OrderPosition> packagePositions, DbTransaction dbTran);
        AccountOrder CreateAccountOrder(int accountId, int orderId, DbTransaction dbTran);
        void UpdateClientAccout(Account account, DbTransaction dbTran);
        void UpdateAccountPosition(AccountPosition accountPosition, DbTransaction dbTran);
        void DeleteAccountPosition(AccountPosition accountPosition, DbTransaction dbTran);
    }
}
