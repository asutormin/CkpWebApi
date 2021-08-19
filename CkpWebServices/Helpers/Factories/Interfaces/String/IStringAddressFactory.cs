using CkpDAL.Entities.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IStringAddressFactory
    {
        StringAddress Create(int stringId, Address address, int orderBy);
    }
}
