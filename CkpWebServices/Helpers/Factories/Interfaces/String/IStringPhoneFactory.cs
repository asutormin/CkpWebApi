using CkpDAL.Model.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IStringPhoneFactory
    {
        StringPhone Create(int stringId, Phone phone, int orderBy);
    }
}
