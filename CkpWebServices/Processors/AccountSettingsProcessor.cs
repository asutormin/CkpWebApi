using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpServices.Helpers.Factories.ClientAccount;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CkpServices.Processors
{
    public class AccountSettingsProcessor : IAccountSettingsProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IAccountSettingsFactory _accountSettingsFactory;

        public AccountSettingsProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;

            _accountSettingsFactory = new AccountSettingsFactory();
        }

        public AccountSettings GetAccountSettingsByLegalPersonId(int clientLegalPersonId)
        {
            var accountSettings = _context.AccountSettings
                .Single(acs => acs.LegalPersonId == clientLegalPersonId);

            return accountSettings;
        }

        public AccountSettings CreateAccountSettings(int accountId, AccountSettings legalPersonAccountSettings, DbTransaction dbTran)
        {
            var accountSettings = _accountSettingsFactory.Create(accountId, legalPersonAccountSettings);
            accountSettings = _repository.SetAccountSettings(accountSettings, isActual: true, dbTran);

            return accountSettings;
        }

        public void UpdateAccountSettings(AccountSettings accountSettings, DbTransaction dbTran)
        {
            _repository.SetAccountSettings(accountSettings, isActual: true, dbTran);
        }
    }
}
