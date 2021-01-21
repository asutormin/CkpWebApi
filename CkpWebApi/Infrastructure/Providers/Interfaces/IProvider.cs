namespace CkpWebApi.Infrastructure.Providers.Interfaces
{
    public interface IProvider<out T>
    {
        T Get();
    }
}
