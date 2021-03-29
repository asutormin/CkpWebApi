using CkpDAL.Model;
using CkpEntities.Input.String;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IStringProcessor
    {
        void CreateFullString(int businessUnitId, int companyId, int orderPositionId, AdvString advString, DbTransaction dbTran);
        bool CanUpdateString(PositionIm positionIm);
        void UpdateFullString(int orderPositionId, AdvString advString, DbTransaction dbTran);
        void DeleteFullString(int orderPositionId, DbTransaction dbTran);
    }
}
