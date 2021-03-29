using CkpDAL.Model.String;
using CkpServices.Helpers.Factories.Interfaces.String;

namespace CkpServices.Helpers.Factories.String
{
    class PhoneFactory : IPhoneFactory
    {
        public Phone Create(int companyId, string countryCode, string code, string number, string description)
        {
            var phone = new Phone
            {
                Id = 0,
                CompanyId = companyId,
                PhoneTypeId = 1,
                CountryCode = countryCode,
                Code = code,
                Number = number,
                AdditionalNumber = null,
                Description = description
            };

            return phone;
        }
    }
}
