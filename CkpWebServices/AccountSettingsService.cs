using CkpDAL;
using CkpDAL.Repository;
using CkpInfrastructure.Configuration;
using CkpServices.Processors;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace CkpServices
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly BPFinanceContext _context;
        private readonly IAccountSettingsProcessor _accountSettingsProcessor;

        public AccountSettingsService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            _accountSettingsProcessor = new AccountSettingsProcessor(_context, repository);
        }

        public bool GetIsNeedPrepayment(int clientLegalPersonId)
        {
            var accountSettings = _accountSettingsProcessor.GetAccountSettingsByLegalPersonId(clientLegalPersonId);

            return accountSettings.IsNeedPrepayment;
        }

        public int GetInteractionBusinessUnitId(int clientLegalPersonId)
        {
            var accountSettings = _accountSettingsProcessor.GetAccountSettingsByLegalPersonId(clientLegalPersonId);

            return accountSettings.InteractionBusinessUnitId;
        }

        public void ResetIsNeedPrepayment(int clientLegalPersonId)
        {
            var accountSettings = _accountSettingsProcessor.GetAccountSettingsByLegalPersonId(clientLegalPersonId);

            if (!accountSettings.IsNeedPrepayment)
            {
                accountSettings.IsNeedPrepayment = true;

                using (var сontextTransaction = _context.Database.BeginTransaction())
                {
                    var dbTran = сontextTransaction.GetDbTransaction();

                    _accountSettingsProcessor.UpdateAccountSettings(accountSettings, dbTran);

                    _context.SaveChanges();
                    dbTran.Commit();
                }
            }
        }

    }
}
