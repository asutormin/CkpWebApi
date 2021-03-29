using CkpDAL.Model.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IAddressFactory
    {
        Address Create(int companyId, string value);
    }
}
