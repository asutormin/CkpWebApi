using CkpDAL.Entities.String;
using CkpModel.Input.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IStringPositionFactory
    {
        StringPosition Create(int businessUnitId, int companyId, int orderPositionId, StringData stringData);
    }
}
