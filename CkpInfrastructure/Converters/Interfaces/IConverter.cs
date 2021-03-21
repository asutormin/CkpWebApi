namespace CkpInfrastructure.Converters.Interfaces
{
    public interface IConverter<in TFrom, out TTo>
    {
        TTo Convert(TFrom fromValue);
    }
}
