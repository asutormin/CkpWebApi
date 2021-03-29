using CkpEntities.Input;

namespace CkpServices.Processors.Interfaces
{
    interface IAdvertisementProcessor
    {
        Advertisement GetAdvertisementFull(int orderPositionId);
    }
}
