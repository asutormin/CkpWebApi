using CkpDAL.Entities.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IPhoneFactory
    {
        Phone Create(int companyId, string countryCode, string code, string number, string additionalNamber, string description);
    }
}
