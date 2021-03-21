using CkpInfrastructure.Providers.Interfaces;
using System.Text.RegularExpressions;

namespace CkpWebApi.Infrastructure.Providers
{
    public sealed class StringWithoutBadSymbolsProvider : IKeyedProvider<string, string>
    {
        private readonly string _replacePatternt;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="badSymbols">Символы, удаляемые из строки.</param>
        public StringWithoutBadSymbolsProvider(params char[] badSymbols)
        {
            _replacePatternt = '[' + string.Concat(badSymbols) + ']';
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="badSymbolsString">Строка содержащая символы, удаляемые из строки.</param>
        public StringWithoutBadSymbolsProvider(string badSymbolsString)
        {
            _replacePatternt = '[' + badSymbolsString + ']';
        }

        /// <summary>
        /// Возвращает строку без недопустимых символов.
        /// </summary>
        /// <param name="value">Обрабатываемая строка.</param>
        /// <returns>Строку без недопустимых символов.</returns>
        public string GetByValue(string value)
        {
            return Regex.Replace(value, _replacePatternt, "");
        }
    }
}
