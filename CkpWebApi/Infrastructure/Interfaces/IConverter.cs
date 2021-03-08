
namespace CkpWebApi.Infrastructure.Interfaces
{
    public interface IConverter<in TFrom, out TTo>
    {
        TTo Convert(TFrom fromValue);
    }
}
