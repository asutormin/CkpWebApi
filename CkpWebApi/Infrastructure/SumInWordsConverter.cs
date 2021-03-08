using CkpWebApi.Infrastructure;
using CkpWebApi.Infrastructure.Interfaces;

namespace CkpWebApi.Helpers.Converters
{
    /// <summary>
    /// Конвертер в сумму прописью.
    /// </summary>
    class SumInWordsConverter : IConverter<decimal, string>
    {
        public string Convert(decimal fromValue)
        {
            return RusCurrency.Str(fromValue, "RUR", false);

        }
    }
}
