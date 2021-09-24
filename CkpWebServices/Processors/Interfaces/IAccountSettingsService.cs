
namespace CkpServices.Processors.Interfaces
{
    public interface IAccountSettingsService
    {
        bool GetIsNeedPrepayment(int clientLegalPersonId);
        void ResetIsNeedPrepayment(int clientLegalPersonId);
    }
}
