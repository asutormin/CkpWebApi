using CkpInfrastructure.Converters.Interfaces;

namespace CkpInfrastructure.Converters
{
    /// <summary>
    /// Конвертер в сумму прописью.
    /// </summary>
    public class SumInWordsConverter : IConverter<decimal, string>
    {
        public string Convert(decimal fromValue)
        {
            return RusCurrency.Str(fromValue, "RUR", false);

        }
    }
}
