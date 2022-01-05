
namespace CkpServices.Processors.Interfaces
{
    public interface IAccountSettingsService
    {
        bool GetIsNeedPrepayment(int clientLegalPersonId);
        int GetInteractionBusinessUnitId(int clientLegalPersonId);
        void ResetIsNeedPrepayment(int clientLegalPersonId);
    }
}
