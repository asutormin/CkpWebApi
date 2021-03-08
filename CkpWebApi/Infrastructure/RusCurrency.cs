using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CkpWebApi.Infrastructure
{
    /// <summary>
    /// Класс для получения строкового представления валюты на русском языке.
    /// </summary>
    public class RusCurrency
    {
        /// <summary>
        /// Кэш валют.
        /// </summary>
        private static readonly HybridDictionary Currencies = new HybridDictionary();

        /// <summary>
        /// Конструктор.
        /// </summary>
        static RusCurrency()
        {
            Register("RUR", true, "рубль", "рубля", "рублей", false, "копейка", "копейки", "копеек");
            Register("EUR", true, "евро", "евро", "евро", true, "евроцент", "евроцента", "евроцентов");
            Register("USD", true, "доллар", "доллара", "долларов", true, "цент", "цента", "центов");
            ConfigurationManager.GetSection(@"currency-names");

        }

        /// <summary>
        /// Регистрация валюты в кэше валют.
        /// </summary>
        /// <param name="currency">Название валюты.</param>
        /// <param name="seniorMale">True - мужской род основной части.</param>
        /// <param name="seniorOne">Строковое предсталение основной части (рубли) валюты для значения 1.</param>
        /// <param name="seniorTwo">Строковое предсталение основной части (рубли) валюты для значений от 2 до 4.</param>
        /// <param name="seniorFive">Строковое предсталение основной части (рубли) валюты для значений  больше 5.</param>
        /// <param name="juniorMale">True - мужской род дополнительной части.</param>
        /// <param name="juniorOne">Строковое предсталение дополнительной (копейки) части валюты для значения 1.</param>
        /// <param name="juniorTwo">Строковое предсталение дополнительной части (копейки) валюты для значений от 2 до 4.</param>
        /// <param name="juniorFive">Строковое предсталение дополнительной части (копейки) валюты для значений  больше 5.</param>
        public static void Register(string currency, bool seniorMale,
            string seniorOne, string seniorTwo, string seniorFive,
            bool juniorMale, string juniorOne, string juniorTwo, string juniorFive)
        {
            CurrencyInfo info;
            info.SeniorMale = seniorMale;
            info.SeniorOne = seniorOne;
            info.SeniorTwo = seniorTwo;
            info.SeniorFive = seniorFive;
            info.JuniorMale = juniorMale;
            info.JuniorOne = juniorOne;
            info.JuniorTwo = juniorTwo;
            info.JuniorFive = juniorFive;
            Currencies.Add(currency, info);
        }

        /// <summary>
        /// Вернуть строковое представление суммы в рублях.
        /// </summary>
        /// <param name="val">Значение суммы.</param>
        /// <param name="juniorString">True - строковое представление копеек, false - числовое.</param>
        public static string Str(decimal val, bool juniorString)
        {
            return Str(val, "RUR", juniorString);
        }

        /// <summary>
        /// Вернуть строковое представление суммы в валюте.
        /// </summary>
        /// <param name="val">Значение суммы.</param>
        /// <param name="currency">Строковое значение валюты.</param>
        /// <param name="juniorString">True - строковое представление копеек, false - числовое.</param>
        /// <exception cref="ArgumentOutOfRangeException">Не определено значение валюты. Параметр - <param name="currency"></param>.</exception>
        public static string Str(decimal val, string currency, bool juniorString)
        {
            if (!Currencies.Contains(currency))
                throw new ArgumentOutOfRangeException("currency", "Валюта \"" + currency + "\" не зарегистрирована");

            var info = (CurrencyInfo)Currencies[currency];
            return Str(val, info.SeniorMale, info.SeniorOne, info.SeniorTwo, info.SeniorFive, juniorString,
                info.JuniorMale, info.JuniorOne, info.JuniorTwo, info.JuniorFive);
        }

        /// <summary>
        /// Вернуть строковое представление суммы в валюте.
        /// </summary>
        /// <param name="val">Значение суммы.</param>
        /// <param name="seniorMale">True - мужской род основной части..</param>
        /// <param name="seniorOne">Строковое предсталение основной части (рубли) валюты для значения 1.</param>
        /// <param name="seniorTwo">Строковое предсталение основной части (рубли) валюты для значений от 2 до 4.</param>
        /// <param name="seniorFive">Строковое предсталение основной части (рубли) валюты для значений  больше 5.</param>
        /// <param name="juniorString">True - строковое представление копеек, false - числовое.</param>
        /// <param name="juniorMale">True - мужской род дополнительной части.</param>
        /// <param name="juniorOne">Строковое предсталение дополнительной (копейки) части валюты для значения 1.</param>
        /// <param name="juniorTwo">Строковое предсталение дополнительной части (копейки) валюты для значений от 2 до 4.</param>
        /// <param name="juniorFive">Строковое предсталение дополнительной части (копейки) валюты для значений  больше 5.</param>
        public static string Str(decimal val, bool seniorMale, string seniorOne, string seniorTwo, string seniorFive,
            bool juniorString, bool juniorMale, string juniorOne, string juniorTwo, string juniorFive)
        {
            var n = (long)Math.Abs(val); //Основная часть (рубли)
            var remainder = (int)((Math.Abs(val) - n + 0.005m) * 100); //Доп. часть (копейки)

            var r = new StringBuilder();
            r.Append(RusNumber.Str(n, seniorMale, seniorOne, seniorTwo, seniorFive));

            if (juniorString)
                r.Append(RusNumber.Str(remainder, juniorMale, juniorOne, juniorTwo, juniorFive).ToLower());
            else
            {
                r.Append(remainder.ToString("00 "));
                r.Append(RusNumber.Case(remainder, juniorOne, juniorTwo, juniorFive));
            }
            return r.ToString().TrimEnd() + " ";
        }

        /// <summary>
        /// Вернуть строковое представление суммы в валюте, разделенное на части.
        /// [0] - основная чать прописью (рубли)
        /// [1] - единицы представления основной части (рубли)
        /// [2] - дополнительная чать число или прописью (копейки)
        /// [3] - единицы представления дополнительной части (копейки)
        /// </summary>
        /// <param name="val">Значение суммы.</param>
        /// <param name="currency">Строковое значение валюты.</param>
        /// <param name="juniorString">True - строковое представление копеек, false - числовое.</param>
        /// <exception cref="ArgumentOutOfRangeException">Не определено значение валюты. Параметр - <param name="currency"></param>.</exception>
        public static string[] Components(decimal val, string currency, bool juniorString)
        {
            if (!Currencies.Contains(currency))
                throw new ArgumentOutOfRangeException("currency", "Валюта \"" + currency + "\" не зарегистрирована");

            var info = (CurrencyInfo)Currencies[currency];
            return Components(val, info.SeniorMale, info.SeniorOne, info.SeniorTwo, info.SeniorFive, juniorString,
                info.JuniorMale, info.JuniorOne, info.JuniorTwo, info.JuniorFive);
        }

        /// <summary>
        /// Вернуть строковое представление суммы в валюте, разделенное на части.
        /// [0] - основная чать прописью (рубли)
        /// [1] - единицы представления основной части (рубли)
        /// [2] - дополнительная чать число или прописью (копейки)
        /// [3] - единицы представления дополнительной части (копейки)
        /// </summary>
        /// <param name="val">Значение суммы.</param>
        /// <param name="seniorMale">True - мужской род основной части..</param>
        /// <param name="seniorOne">Строковое предсталение основной части (рубли) валюты для значения 1.</param>
        /// <param name="seniorTwo">Строковое предсталение основной части (рубли) валюты для значений от 2 до 4.</param>
        /// <param name="seniorFive">Строковое предсталение основной части (рубли) валюты для значений  больше 5.</param>
        /// <param name="juniorString">True - строковое представление копеек, false - числовое.</param>
        /// <param name="juniorMale">True - мужской род дополнительной части.</param>
        /// <param name="juniorOne">Строковое предсталение дополнительной (копейки) части валюты для значения 1.</param>
        /// <param name="juniorTwo">Строковое предсталение дополнительной части (копейки) валюты для значений от 2 до 4.</param>
        /// <param name="juniorFive">Строковое предсталение дополнительной части (копейки) валюты для значений  больше 5.</param>
        public static string[] Components(decimal val, bool seniorMale, string seniorOne, string seniorTwo, string seniorFive,
            bool juniorString, bool juniorMale, string juniorOne, string juniorTwo, string juniorFive)
        {
            var n = (long)Math.Abs(val); //Основная часть (рубли)
            var remainder = (int)((Math.Abs(val) - n + 0.005m) * 100); //Доп. часть (копейки)

            var seniorStr = RusNumber.Components(n, seniorMale, seniorOne, seniorTwo, seniorFive);

            var juniorStr = juniorString
                ? RusNumber.Components(remainder, juniorMale, juniorOne, juniorTwo, juniorFive)
                : new[] { remainder.ToString("00"), RusNumber.Case(remainder, juniorOne, juniorTwo, juniorFive) };
            juniorStr[0] = juniorStr[0].ToLower();

            var result = new List<string>();
            result.AddRange(seniorStr);
            result.AddRange(juniorStr);
            return result.ToArray();
        }
    };
}
