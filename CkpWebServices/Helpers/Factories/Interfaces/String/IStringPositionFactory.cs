using CkpDAL.Model.String;
using CkpEntities.Input.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IStringPositionFactory
    {
        StringPosition Create(int businessUnitId, int companyId, int orderPositionId, AdvString advString);
    }
}
