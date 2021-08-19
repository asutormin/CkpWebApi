using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpServices.Helpers.Factories.ClientAccount;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using CkpServices.Processors.Interfaces;
using System.Data.Common;

namespace CkpServices.Processors
{
    class ClientAccountProcessor : IClientAccountProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IClientAccountFactory _accountFactory;
        private readonly IAccountPositionFactory _accountPositionFactory;
        private readonly IAccountSettingsFactory _accountSettingsFactory;
        private readonly IAccountOrderFactory _accountOrderFactory;

        public ClientAccountProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;            

            _accountFactory = new ClientAccountFactory();
            _accountPositionFactory = new AccountPositionFactory();
            _accountSettingsFactory = new AccountSettingsFactory();
            _accountOrderFactory = new AccountOrderFactory();
        }

        public Account CreateClientAccount(int supplierLegalPersonId, float sum, Order basketOrder, DbTransaction dbTran)
        {
            var accountNumber = _repository.GetAccountNumber(supplierLegalPersonId, dbTran);

            var account = _accountFactory.Create(accountNumber, sum, basketOrder.BusinessUnit, basketOrder.ClientLegalPerson);
            account = _repository.SetAccount(account, isActual: true, dbTran);

            return account;
        }

        public AccountPosition CreateAccountPosition(int accountId, OrderPosition orderPosition, DbTransaction dbTran)
        {
            var accountPosition = _accountPositionFactory.Create(accountId, orderPosition);
            accountPosition =_repository.SetAccountPosition(accountPosition, isActual: true, dbTran);

            return accountPosition;
        }

        public AccountSettings CreateAccountSettings(int accountId, AccountSettings legalPersonAccountSettings, DbTransaction dbTran)
        {
            var accountSettings = _accountSettingsFactory.Create(accountId, legalPersonAccountSettings);
            accountSettings = _repository.SetAccountSettings(accountSettings, isActual: true, dbTran);

            return accountSettings;
        }

        public AccountOrder CreateAccountOrder(int accountId, int orderId, DbTransaction dbTran)
        {
            var accountOrder = _accountOrderFactory.Create(accountId, orderId);
            accountOrder = _repository.SetAccountOrder(accountOrder, isActual: true, dbTran);

            return accountOrder;
        }
    }
}
