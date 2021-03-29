using CkpDAL.Model;
using CkpEntities.Input;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IPositionImProcessor
    {
        void CreatePositionIm(int businessUnitId, int orderId, int orderPositionId, Advertisement adv, DbTransaction dbTran);
        void UpdatePositionIm(PositionIm positionIm, Advertisement adv, DbTransaction dbTran);
        PositionIm UpdatePositionIm(PositionIm positionIm, int orderId, int maketStatusId, DbTransaction dbTran);
        void DeletePositionIm(PositionIm positionIm, DbTransaction dbTran);
    }
}
