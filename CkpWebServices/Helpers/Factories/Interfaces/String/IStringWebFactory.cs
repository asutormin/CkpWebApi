using CkpDAL.Entities.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IStringWebFactory
    {
        StringWeb Create(int stringId, Web web, int orderBy);
    }
}
