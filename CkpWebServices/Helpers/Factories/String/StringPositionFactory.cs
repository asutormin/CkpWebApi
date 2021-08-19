using CkpDAL.Entities.String;
using CkpModel.Input.String;
using CkpServices.Helpers.Converters;
using CkpServices.Helpers.Factories.Interfaces.String;
using System;

namespace CkpServices.Helpers.Factories.String
{
    class StringPositionFactory : IStringPositionFactory
    {
        public StringPosition Create(int businessUnitId, int companyId, int orderPositionId, StringData stringData)
        {
            var stringPosition = new StringPosition
            {
                Id = 0, // Идентификатор строки
                CompanyId = companyId, // Идентификатор компании
                OrderPositionId = orderPositionId,
                AnonymousCompanyName = stringData.AnonymousCompanyName, // Название компании для анонимных вакансий
                BusinessUnitId = businessUnitId, // Идентификатор бизнес еденицы
                Date = null, // Дата создания строки
                VacancyName = stringData.VacancyName, // Название вакансии
                VacancyAdditional = stringData.VacancyAdditional, // Дополнение к названию вакансии
                VacancyCode = null, // Код вакансии
                Responsibility = stringData.Responsibility, // Обязаности
                Requirement = stringData.RequirementsData.Value, // Требования
                AgeFrom = stringData.RequirementsData.AgeData.From, // Возраст от
                AgeTo = stringData.RequirementsData.AgeData.To,// Возраст до
                GenderId = stringData.RequirementsData.GenderId, // Идентификатор пола
                EducationId = stringData.RequirementsData.EducationId, // Идентификатор образования
                ExperienceId = stringData.RequirementsData.ExperienceData.Id, // Идентификатор опыта работы
                ExperienceValue = stringData.RequirementsData.ExperienceData.Value, // Значение опыта
                CitizenshipId = stringData.RequirementsData.CitizenshipId, // Идентификатор гражданства
                Condition = stringData.ConditionsData.Value, // Условия
                SalaryFrom = stringData.ConditionsData.SalaryData.From, // Доход от
                SalaryTo = stringData.ConditionsData.SalaryData.To, // Доход до
                SalaryDescription = stringData.ConditionsData.SalaryData.Description, // Примечание к уровню дохода
                CurrencyId = stringData.ConditionsData.SalaryData.CurrencyId, // Идентификатор валюты
                WorkGraphicId = stringData.ConditionsData.WorkGraphicData.Id, // Идентификатор графика работы
                WorkGraphic = stringData.ConditionsData.WorkGraphicData.Comment,
                Keywords = null, // Ключевые слова
                Logo = Base64ToBytesConverter.Convert(stringData.LogoData.Base64String),
                LogoFileName = stringData.LogoData.FileName,
                ContactFirstName = stringData.ContactData.FirstName, // Имя контактного лица
                ContactSecondName = stringData.ContactData.SecondName, // Оотчество контактного лица
                ContactLastName = stringData.ContactData.LastName, // Фамилия контактного лица
                Text = null, // изначальный текст
                IsSalaryPercent = stringData.ConditionsData.SalaryData.IsSalaryPercent, // Процент с продаж
                IsHousing = stringData.ConditionsData.IsHousing, // Предоставляется общежитие
                IsFood = stringData.ConditionsData.IsFood, // Предоставляются обеды
                BeginDate = DateTime.Now
            };

            return stringPosition;
        }
    }
}
