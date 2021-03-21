
namespace CkpInfrastructure
{
    /// <summary>
    /// Структура представления валюты
    /// </summary>
    struct CurrencyInfo
    {
        /// <summary>
        /// Мужской род основной части.
        /// </summary>
        public bool SeniorMale;

        /// <summary>
        /// Строковое предсталение основной части (рубли) валюты для значения 1.
        /// </summary>
        public string SeniorOne;

        /// <summary>
        /// Строковое предсталение основной части (рубли) валюты для значений от 2 до 4.
        /// </summary>
        public string SeniorTwo;

        /// <summary>
        /// Строковое предсталение основной части (рубли) валюты для значений  больше 5.
        /// </summary>
        public string SeniorFive;

        /// <summary>
        /// Мужской род дополнительной части.
        /// </summary>
        public bool JuniorMale;

        /// <summary>
        /// Строковое предсталение дополнительной (копейки) части валюты для значения 1.
        /// </summary>
        public string JuniorOne;

        /// <summary>
        /// Строковое предсталение дополнительной части (копейки) валюты для значений от 2 до 4.
        /// </summary>
        public string JuniorTwo;

        /// <summary>
        /// Строковое предсталение дополнительной части (копейки) валюты для значений  больше 5.
        /// </summary>
        public string JuniorFive;
    };
}
