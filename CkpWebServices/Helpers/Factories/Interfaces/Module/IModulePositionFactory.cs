using CkpDAL.Entities;
using CkpDAL.Entities.Module;

namespace CkpServices.Helpers.Factories.Interfaces.Module
{
    interface IModulePositionFactory
    {
        ModulePosition Create(int companyId, int businessUnitId, int orderPositionId);
    }
}
