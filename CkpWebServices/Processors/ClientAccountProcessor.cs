using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpServices.Helpers.Factories.ClientAccount;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using CkpServices.Processors.Interfaces;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors
{
    class ClientAccountProcessor : IClientAccountProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IClientAccountFactory _accountFactory;
        private readonly IAccountPositionFactory _accountPositionFactory;
        private readonly IAccountOrderFactory _accountOrderFactory;

        public ClientAccountProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;            

            _accountFactory = new ClientAccountFactory();
            _accountPositionFactory = new AccountPositionFactory();
            _accountOrderFactory = new AccountOrderFactory();
        }

        public Account CreateClientAccount(int supplierLegalPersonId, float sum, Order basketOrder, DbTransaction dbTran)
        {
            var accountNumber = _repository.GetAccountNumber(supplierLegalPersonId, dbTran);

            var account = _accountFactory.Create(accountNumber, sum, basketOrder);
            account = _repository.SetAccount(account, isActual: true, dbTran);

            return account;
        }

        public AccountPosition CreateAccountPosition(int accountId, OrderPosition orderPosition, List<OrderPosition> packagePositions, DbTransaction dbTran)
        {
            var accountPosition = _accountPositionFactory.Create(accountId, orderPosition, packagePositions);
            accountPosition =_repository.SetAccountPosition(accountPosition, isActual: true, dbTran);

            return accountPosition;
        }

        public AccountOrder CreateAccountOrder(int accountId, int orderId, DbTransaction dbTran)
        {
            var accountOrder = _accountOrderFactory.Create(accountId, orderId);
            accountOrder = _repository.SetAccountOrder(accountOrder, isActual: true, dbTran);

            return accountOrder;
        }

        public void UpdateClientAccout(Account account, DbTransaction dbTran)
        {
            _repository.SetAccount(account, isActual: true, dbTran);
        }

        public void UpdateAccountPosition(AccountPosition accountPosition, DbTransaction dbTran)
        {
            _repository.SetAccountPosition(accountPosition, isActual: true, dbTran);
        }

        public void DeleteAccountPosition(AccountPosition accountPosition, DbTransaction dbTran)
        {
            _repository.SetAccountPosition(accountPosition, isActual: false, dbTran);
        }
    }
}
