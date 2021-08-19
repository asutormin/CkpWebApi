using CkpModel.Input;
using CkpModel.Output;

namespace CkpServices.Interfaces
{
    public interface IUserService
    {
        AuthInfo Authenticate(AuthData auth);
        AuthInfo GetByClientLegalPersonId(int clientLegalPersonId);
        bool CanAccessOrderPosition(int clientLegalPersonId, int orderPositionId);
        bool CanAccessAccount(int clientLegalPersonId, int clientAccountId);
    }
}
