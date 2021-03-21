using System;
using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        void SetString(
            DbTransaction dbTran,
            ref int id, // Идентификатор строки
            int parentId,
            int companyId, // Идентификатро компании
            string anonymousCompanyName, // Название компании для анонимных вакансий
            int businessUnitId, // Идентификатор бизнес еденицы
            DateTime? date, // Дата создания строки
            string vacancyName, // Название вакансии
            string vacancyAdditional, // Дополнение к названию вакансии
            string vacancyCode, // Код вакансии
            string responsibility, // Обязаности
            string requirement, // Требования
            string condition, // Условия
            float? salaryFrom, // Доход от
            float? salaryTo, // Доход до
            string salaryDescription, // Примечание к уровню дохода
            int? currencyId, // Идентификатор валюты
            int? workGraphicId, // Идентификатор графика работы
            string workGraphicComm,
            int? ageFrom, // Возраст от
            int? ageTo, // Возраст до
            int? genderId, // Идентификатор пола
            int? educationId, // Идентификатор образования
            int? experienceId, // Идентификатор опыта работы
            int experienceValue, // Значение опыта
            int? citizenshipId, // Идентификатор гражданства
            string keywords, // Ключевые слова
            byte[] logo,
            string logoFileName,
            string contactFirstName, // имя контактного лица
            string contactSecondName, // отчество контактного лица
            string contactLastName, // фамилия контактного лица
            string text, // изначальный текст
            int orderPositionId,
            bool isSalaryPercent, // Процент с продаж
            bool isHousing, // Предоставляется общежитие
            bool isFood, // Предоставляются обеды
            int editUserId, // пользователь изменивший запись
            bool isActual, // Закрыть или обновить запись
            ref DateTime lastEditDate);
    }
}
