namespace CkpServices.Processors.Interfaces
{
    interface IClientProcessor
    {
        float GetDiscount(int legalPersonId, int businessUnitId, int supplierId, int pricePositionTypeId);
    }
}
