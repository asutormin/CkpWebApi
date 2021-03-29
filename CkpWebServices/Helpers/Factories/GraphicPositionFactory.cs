using CkpDAL.Model;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class GraphicPositionFactory : IGraphicPositionFactory
    {
        public GraphicPosition Create(int orderPositionId, int parentGraphicPositionId, int graphicId)
        {
            var graphicPosition = new GraphicPosition
            {
                Id = 0,
                OrderPositionId = orderPositionId,
                ParenGraphicPositiontId = parentGraphicPositionId,
                Count = 1,
                GraphicId = graphicId,
                BeginDate = DateTime.Now
            };

            return graphicPosition;
        }
    }
}
