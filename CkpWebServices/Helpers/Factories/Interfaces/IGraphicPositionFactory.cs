using CkpDAL.Model;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IGraphicPositionFactory
    {
        GraphicPosition Create(int orderPositionId, int parentGraphicPositionId, int graphicId);
    }
}
