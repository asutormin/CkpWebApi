using CkpDAL.Entities;
using CkpModel.Input.String;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IStringProcessor
    {
        void CreateFullString(int businessUnitId, int companyId, int orderPositionId, StringData stringData, DbTransaction dbTran);
        bool CanUpdateString(PositionIm positionIm);
        void UpdateFullString(int orderPositionId, StringData stringData, DbTransaction dbTran);
        void DeleteFullString(int orderPositionId, DbTransaction dbTran);
    }
}
