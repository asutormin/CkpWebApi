using System;
using System.Linq;
using System.Text;

namespace CkpWebApi.Infrastructure
{
    /// <summary>
    /// Клас для строкового представления числа от 0 до 999 на русском языке.
    /// </summary>
    public class RusNumber
    {
        /// <summary>
        /// Строковые представления сотен.
        /// </summary>
        private static readonly string[] Hunds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        /// <summary>
        /// Строковые представления десяток.
        /// </summary>
        private static readonly string[] Tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        /// <summary>
        /// Стрковые представления первых двадцати чисел в мужском роде.
        /// </summary>
        private static readonly string[] FracMale20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

        /// <summary>
        /// Стрковые представления первых двадцати чисел в женском роде.
        /// </summary>
        private static readonly string[] FracFeminine20 =
            {
                "", "одна ", "две ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

        /// <summary>
        /// Вернуть строковое представления числа с единцой измерения.
        /// </summary>
        /// <param name="val">Значение.</param>
        /// <param name="male">True - мужской род.</param>
        /// <param name="one">Строковое предсталение удиницы измерения для значения 1.</param>
        /// <param name="two">Строковое предсталение единицы измерения для значений от 2 до 4.</param>
        /// <param name="five">Строковое предсталение удиницы измерений для значений  больше 5.</param>
        /// <exception cref="ArgumentOutOfRangeException">Параметр не должен быть отрицательным.</exception>
        private static string Str999(long val, bool male, string one = "", string two = "", string five = "")
        {
            var num = (int)(val % 1000);
            if (0 == num) return "";
            if (num < 0) throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
            var frac = male ? FracMale20 : FracFeminine20;

            var r = new StringBuilder(Hunds[num / 100]);

            if (num % 100 < 20)
            {
                r.Append(frac[num % 100]);
            }
            else
            {
                r.Append(Tens[num % 100 / 10]);
                r.Append(frac[num % 10]);
            }

            var unit = Case(num, one, two, five);
            if (!string.IsNullOrEmpty(unit))
            {
                r.Append(unit).Append(" ");
            }
            return r.ToString();
        }

        /// <summary>
        /// Вернуть строковое представления числа с единцой измерения.
        /// </summary>
        /// <param name="val">Значение.</param>
        /// <param name="male">True - мужской род.</param>
        /// <param name="one">Строковое предсталение удиницы измерения для значения 1.</param>
        /// <param name="two">Строковое предсталение единицы измерения для значений от 2 до 4.</param>
        /// <param name="five">Строковое предсталение удиницы измерений для значений  больше 5.</param>
        public static string Str(long val, bool male, string one = "", string two = "", string five = "")
        {
            if (val == 0)
                return string.Format("Ноль {0} ", five).TrimEnd() + " ";

            var n = Math.Abs(val);

            var r = new StringBuilder();

            r.Append(n % 1000 != 0 ? Str999(n, male, one, two, five) : five);

            n /= 1000;

            r.Insert(0, Str999(n, false, "тысяча", "тысячи", "тысяч"));
            n /= 1000;

            r.Insert(0, Str999(n, true, "миллион", "миллиона", "миллионов"));
            n /= 1000;

            r.Insert(0, Str999(n, true, "миллиард", "миллиарда", "миллиардов"));
            n /= 1000;

            r.Insert(0, Str999(n, true, "триллион", "триллиона", "триллионов"));
            n /= 1000;

            r.Insert(0, Str999(n, true, "триллиард", "триллиарда", "триллиардов"));

            if (val < 0)
                r.Insert(0, "минус ");

            //Делаем первую букву заглавной
            r[0] = char.ToUpper(r[0]);

            return r.ToString().TrimEnd() + " ";
        }

        /// <summary>
        /// Возвращает необходимую единицу измерения из переданных, для числа.
        /// </summary>
        /// <param name="val">Значение.</param>
        /// <param name="one">Строковое предсталение удиницы измерения для значения 1.</param>
        /// <param name="two">Строковое предсталение единицы измерения для значений от 2 до 4.</param>
        /// <param name="five">Строковое предсталение удиницы измерений для значений  больше 5.</param>
        public static string Case(int val, string one, string two, string five)
        {
            var t = (val % 100 > 20) ? val % 10 : val % 20;

            switch (t)
            {
                case 1: return one;
                case 2:
                case 3:
                case 4: return two;
                default: return five;
            }
        }

        /// <summary>
        /// Вернуть строковое представления числа с единцой измерения.
        /// </summary>
        /// <param name="val">Значение.</param>
        /// <param name="male">True - мужской род.</param>
        /// <param name="one">Строковое предсталение единицы измерения для значения 1.</param>
        /// <param name="two">Строковое предсталение единицы измерения для значений от 2 до 4.</param>
        /// <param name="five">Строковое предсталение единицы измерений для значений  больше 5.</param>
        /// <returns>string[2]. [0] - содержит строковое представление числа. [1] - содержит строковое представление единицы измерения.</returns>
        public static string[] Components(long val, bool male, string one = "", string two = "", string five = "")
        {
            if (val == 0) return new[] { "Ноль", five };

            var isNegative = val < 0;
            val = Math.Abs(val);

            var unit = Case((int)(val % 1000), one, two, five);

            var r = new StringBuilder();

            r.Append(val % 1000 != 0 ? Str999(val, male) : "");

            val /= 1000;

            r.Insert(0, Str999(val, false, "тысяча", "тысячи", "тысяч"));
            val /= 1000;

            r.Insert(0, Str999(val, true, "миллион", "миллиона", "миллионов"));
            val /= 1000;

            r.Insert(0, Str999(val, true, "миллиард", "миллиарда", "миллиардов"));
            val /= 1000;

            r.Insert(0, Str999(val, true, "триллион", "триллиона", "триллионов"));
            val /= 1000;

            r.Insert(0, Str999(val, true, "триллиард", "триллиарда", "триллиардов"));

            if (isNegative)
                r.Insert(0, "минус ");

            //Делаем первую букву заглавной
            r[0] = char.ToUpper(r[0]);

            return new[] { r.ToString().Trim(), unit };
        }
    };
}
