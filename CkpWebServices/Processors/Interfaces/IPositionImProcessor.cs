using CkpDAL.Entities;
using CkpModel.Input;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IPositionImProcessor
    {
        void CreatePositionIm(int businessUnitId, int orderId, int orderPositionId, OrderPositionData opd, DbTransaction dbTran);
        void UpdatePositionIm(PositionIm positionIm, OrderPositionData opd, DbTransaction dbTran);
        PositionIm UpdatePositionIm(PositionIm positionIm, int orderId, int maketStatusId, DbTransaction dbTran);
        void DeletePositionIm(PositionIm positionIm, DbTransaction dbTran);
    }
}
