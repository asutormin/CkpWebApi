using CkpDAL.Model.String;
using CkpServices.Helpers.Factories.Interfaces.String;

namespace CkpServices.Helpers.Factories.String
{
    class StringAddressFactory : IStringAddressFactory
    {
        public StringAddress Create(int stringId, Address address, int orderBy)
        {
            var stringAddress = new StringAddress
            {
                StringId = stringId,
                AddressId = address.Id,
                CityId = address.CityId,
                MetroId = address.MetroId,
                Street = address.Street,
                House = address.House,
                Corps = address.Corps,
                Building = address.Building,
                Description = address.Description,
                OrderBy = orderBy
            };

            return stringAddress;
        }
    }
}
