using CkpDAL.Entities.String;
using CkpServices.Helpers.Factories.Interfaces.String;

namespace CkpServices.Helpers.Factories.String
{
    class StringPhoneFactory : IStringPhoneFactory
    {
        public StringPhone Create(int stringId, Phone phone, int orderBy)
        {
            var stringPhone = new StringPhone
            {
                StringId = stringId,
                PhoneId = phone.Id,
                PhoneTypeId = phone.PhoneTypeId,
                CountryCode = phone.CountryCode,
                Code = phone.Code,
                Number = phone.Number,
                AdditionalNumber = phone.AdditionalNumber,
                Description = phone.Description,
                OrderBy = orderBy
            };

            return stringPhone;
        }
    }
}
