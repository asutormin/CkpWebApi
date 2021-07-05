using CkpDAL.Model.String;
using CkpEntities.Input.String;
using CkpServices.Helpers.Converters;
using CkpServices.Helpers.Factories.Interfaces.String;
using System;

namespace CkpServices.Helpers.Factories.String
{
    class StringPositionFactory : IStringPositionFactory
    {
        public StringPosition Create(int businessUnitId, int companyId, int orderPositionId, AdvString advString)
        {
            var stringPosition = new StringPosition
            {
                Id = 0, // Идентификатор строки
                CompanyId = companyId, // Идентификатор компании
                OrderPositionId = orderPositionId,
                AnonymousCompanyName = advString.AnonymousCompanyName, // Название компании для анонимных вакансий
                BusinessUnitId = businessUnitId, // Идентификатор бизнес еденицы
                Date = null, // Дата создания строки
                VacancyName = advString.VacancyName, // Название вакансии
                VacancyAdditional = advString.VacancyAdditional, // Дополнение к названию вакансии
                VacancyCode = null, // Код вакансии
                Responsibility = advString.Responsibility, // Обязаности
                Requirement = advString.Requirements.Value, // Требования
                AgeFrom = advString.Requirements.Age.From, // Возраст от
                AgeTo = advString.Requirements.Age.To,// Возраст до
                GenderId = advString.Requirements.GenderId, // Идентификатор пола
                EducationId = advString.Requirements.EducationId, // Идентификатор образования
                ExperienceId = advString.Requirements.Experience.Id, // Идентификатор опыта работы
                ExperienceValue = advString.Requirements.Experience.Value, // Значение опыта
                CitizenshipId = advString.Requirements.CitizenshipId, // Идентификатор гражданства
                Condition = advString.Conditions.Value, // Условия
                SalaryFrom = advString.Conditions.Salary.From, // Доход от
                SalaryTo = advString.Conditions.Salary.To, // Доход до
                SalaryDescription = advString.Conditions.Salary.Description, // Примечание к уровню дохода
                CurrencyId = advString.Conditions.Salary.CurrencyId, // Идентификатор валюты
                WorkGraphicId = advString.Conditions.WorkGraphic.Id, // Идентификатор графика работы
                WorkGraphic = advString.Conditions.WorkGraphic.Comment,
                Keywords = null, // Ключевые слова
                Logo = Base64ToBytesConverter.Convert(advString.Logo.Base64String),
                LogoFileName = advString.Logo.FileName,
                ContactFirstName = advString.Contact.FirstName, // Имя контактного лица
                ContactSecondName = advString.Contact.SecondName, // Оотчество контактного лица
                ContactLastName = advString.Contact.LastName, // Фамилия контактного лица
                Text = null, // изначальный текст
                IsSalaryPercent = advString.Conditions.Salary.IsSalaryPercent, // Процент с продаж
                IsHousing = advString.Conditions.IsHousing, // Предоставляется общежитие
                IsFood = advString.Conditions.IsFood, // Предоставляются обеды
                BeginDate = DateTime.Now
            };

            return stringPosition;
        }
    }
}
