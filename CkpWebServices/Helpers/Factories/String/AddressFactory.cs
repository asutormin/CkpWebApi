using CkpDAL.Model.String;
using CkpServices.Helpers.Factories.Interfaces.String;

namespace CkpServices.Helpers.Factories.String
{
    class AddressFactory : IAddressFactory
    {
        public Address Create(int companyId, string value)
        {
            var address = new Address
            {
                Id = 0,
                CompanyId = companyId,
                CityId = null,
                MetroId = null,
                Street = null, // Улица
                House = null, // Номер дома
                Corps = null, // Корпус
                Building = null, // Строение
                Description = value
            };

            return address;
        }
    }
}
