namespace CkpInfrastructure.Providers.Interfaces
{
    public interface IProvider<out T>
    {
        T Get();
    }
}
