using CkpDAL.Entities;

namespace CkpServices.Helpers.Factories.Interfaces
{
    interface IPositionImFactory
    {
        PositionIm Create(int orderId, int orderPositionId, int positionImTypeId);
    }
}
