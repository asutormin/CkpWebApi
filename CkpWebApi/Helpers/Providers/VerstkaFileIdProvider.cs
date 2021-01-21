using CkpWebApi.Infrastructure.Providers.Interfaces;

namespace CkpWebApi.Helpers.FileNameProviders
{
    public sealed class VerstkaFileIdProvider : IKeyedProvider<int, string>
    {
        private readonly int _symbolCount;

        public VerstkaFileIdProvider(int symbolCount)
        {
            _symbolCount = symbolCount;
        }

        public string GetByValue(int orderPositionId)
        {
            var orderPositionIdString = orderPositionId.ToString("D");

            return orderPositionIdString.Substring(orderPositionIdString.Length - _symbolCount);
        }
    }
}
