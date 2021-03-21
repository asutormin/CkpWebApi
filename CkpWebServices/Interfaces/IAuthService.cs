using CkpEntities.Output;

namespace CkpServices.Interfaces
{
    public interface IAuthService
    {
        AuthInfo Authenticate(string login, string password, bool createMd5);

        AuthInfo SetLogin(string oldLogin, string newLogin);

        AuthInfo SetPassword(string login, string newPassword);
    }
}
