using CkpDAL.Model.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IPhoneFactory
    {
        Phone Create(int companyId, string countryCode, string code, string number, string description);
    }
}
