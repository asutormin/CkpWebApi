
namespace CkpWebApi.Infrastructure.Interfaces
{
    public interface IBuilder<T>
    {
        void Build();
        T GetResult();
    }
}
