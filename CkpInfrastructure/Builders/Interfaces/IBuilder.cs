namespace CkpInfrastructure.Builders.Interfaces
{
    public interface IBuilder<T>
    {
        void Build();
        T GetResult();
    }
}
