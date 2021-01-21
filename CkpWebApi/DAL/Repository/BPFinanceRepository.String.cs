using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace CkpWebApi.DAL.Repository
{
    public partial class BPFinanceRepository
    {
        public void SetString(
            DbTransaction dbTran,
            ref int stringId, // Идентификатор строки
            int parentId,
            int companyId, // Идентификатро компании
            string anonymousCompanyName, // Навзание компании для анонимных вакансий
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
            ref DateTime lastEditDate) // Дата последнего редактирования записи
        {
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "spSetString";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = dbTran;

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = stringId
                    });

                cmd.Parameters.Add(
                new SqlParameter
                {
                    ParameterName = "@ParentId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    SqlValue = parentId
                });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = companyId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AnonymousCompanyName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = string.IsNullOrEmpty(anonymousCompanyName) ? DBNull.Value : (object)anonymousCompanyName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@BusinessUnitId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = businessUnitId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Date",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)date ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@VacancyName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 50,
                        SqlValue = vacancyName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@VacancyAdditional",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = vacancyAdditional
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@VacancyCode",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 5,
                        SqlValue = string.IsNullOrEmpty(vacancyCode) ? DBNull.Value : (object)vacancyCode
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Responsibility",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 2000,
                        SqlValue = string.IsNullOrEmpty(responsibility) ? DBNull.Value : (object)responsibility
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Requirement",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 2000,
                        SqlValue = string.IsNullOrEmpty(requirement) ? DBNull.Value : (object)requirement
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Condition",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 2000,
                        SqlValue = string.IsNullOrEmpty(condition) ? DBNull.Value : (object)condition
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@SalaryFrom",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)salaryFrom ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@SalaryTo",
                        SqlDbType = SqlDbType.Money,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)salaryTo ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@SalaryDescription",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = string.IsNullOrEmpty(salaryDescription) ? DBNull.Value : (object)salaryDescription
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CurrencyId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)currencyId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WorkGraphicId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)workGraphicId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@WorkGraphicComm",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 256,
                        SqlValue = workGraphicComm
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AgeFrom",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)ageFrom ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@AgeTo",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)ageTo ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@GenderId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)genderId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EducationId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)educationId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ExperienceId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)experienceId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ExperienceValue",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = experienceValue
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CitizenshipId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)citizenshipId ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Keywords",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 300,
                        SqlValue = string.IsNullOrEmpty(keywords) ? DBNull.Value : (object)keywords
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Logo",
                        SqlDbType = SqlDbType.Image,
                        Direction = ParameterDirection.Input,
                        SqlValue = (object)logo ?? DBNull.Value
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LogoFileName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = string.IsNullOrEmpty(logoFileName) ? DBNull.Value : (object)logoFileName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ContactFirstName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = string.IsNullOrEmpty(contactFirstName) ? DBNull.Value : (object)contactFirstName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ContactSecondName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = string.IsNullOrEmpty(contactSecondName) ? DBNull.Value : (object)contactSecondName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@ContactLastName",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Size = 100,
                        SqlValue = string.IsNullOrEmpty(contactLastName) ? DBNull.Value : (object)contactLastName
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@Text",
                        SqlDbType = SqlDbType.Text,
                        Direction = ParameterDirection.Input,
                        SqlValue = text
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@OrderPositionId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = orderPositionId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsSalaryPercent",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isSalaryPercent
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsHousing",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isHousing
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsFood",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isFood
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@IsActual",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        SqlValue = isActual
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@EditUserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        SqlValue = editUserId
                    });

                cmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@LastEditDate",
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.InputOutput,
                        SqlValue = lastEditDate
                    });

                cmd.ExecuteNonQuery();

                stringId = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                lastEditDate = Convert.ToDateTime(cmd.Parameters["@LastEditDate"].Value);
            }
        }
    }
}
