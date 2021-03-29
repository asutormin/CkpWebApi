using CkpDAL.Model;
using CkpEntities.Input;
using System.Collections.Generic;
using System.Data.Common;

namespace CkpServices.Processors.Interfaces
{
    interface IGraphicProcessor
    {
        List<AdvertisementGraphic> GetPackageAdvGraphics(Advertisement adv);
        void CreateGraphicPositions(int orderPositionId, List<AdvertisementGraphic> advGraphics, DbTransaction dbTran);
        void UpdateGraphicPositions(int orderPositionId, IEnumerable<GraphicPosition> graphicPositions, List<AdvertisementGraphic> advGraphics, DbTransaction dbTran);
        void DeleteGraphicPositions(IEnumerable<GraphicPosition> graphicPositions, DbTransaction dbTran);
    }
}
