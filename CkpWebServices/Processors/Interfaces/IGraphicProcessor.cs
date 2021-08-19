using CkpDAL.Entities;
using CkpModel.Input;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IGraphicProcessor
    {
        List<GraphicData> GetPackageGraphicsData(OrderPositionData opd);
        void CreateGraphicPositions(int orderPositionId, List<GraphicData> graphicsData, DbTransaction dbTran);
        void UpdateGraphicPositions(int orderPositionId, IEnumerable<GraphicPosition> graphicPositions, List<GraphicData> advGraphics, DbTransaction dbTran);
        void DeleteGraphicPositions(IEnumerable<GraphicPosition> graphicPositions, DbTransaction dbTran);
    }
}
