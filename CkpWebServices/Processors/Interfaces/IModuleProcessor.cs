using CkpDAL.Entities.Module;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IModuleProcessor
    {
        ModulePosition CreateModule(int clientId, int businessUnitId, int orderPositionId, DbTransaction dbTran);
        void DeleteModule(int orderPositionId, DbTransaction dbTran);
    }
}
