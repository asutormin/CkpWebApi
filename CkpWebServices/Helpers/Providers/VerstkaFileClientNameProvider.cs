using CkpInfrastructure;
using CkpInfrastructure.Providers.Interfaces;

namespace CkpServices.Helpers.Providers
{
    /// <summary>
    /// Возвращает имя клиента для формирования названия файла выгрузки
    /// </summary>
    public sealed class VerstkaFileClientNameProvider : IKeyedProvider<string, string>
    {
        private readonly IKeyedProvider<string, string> _stringWithoutBadSymbolsProvider;
        private readonly int _maxLength;

        public VerstkaFileClientNameProvider(
            IKeyedProvider<string, string> stringWithoutBadSymbolsProvider, 
            int maxLength = 0)
        {
            _stringWithoutBadSymbolsProvider = stringWithoutBadSymbolsProvider;
            _maxLength = maxLength;
        }

        public string GetByValue(string clientName)
        {
            var transliterateClientName = clientName.Front();
            var clientNameWithoutBadSymbols = _stringWithoutBadSymbolsProvider.GetByValue(transliterateClientName);
            var clientNameForVerstkaFile =
                _maxLength == 0
                ? clientNameWithoutBadSymbols
                : clientNameWithoutBadSymbols.Substring(0, _maxLength);

            return clientNameForVerstkaFile;
        }
    }
}
