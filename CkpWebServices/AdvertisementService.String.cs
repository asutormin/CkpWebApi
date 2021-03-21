using CkpDAL.Model;
using CkpDAL.Model.String;
using CkpEntities.Input.String;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        #region Create

        public int CreateString(int companyId, int orderPositionId, AdvString advString, DbTransaction dbTran)
        {
            var stringId = 0;
            var logoBytes = advString.Logo.Base64String == null ? null : Encoding.ASCII.GetBytes(advString.Logo.Base64String);
            var lastEditDate = DateTime.Now;

            _repository.SetString(
                dbTran: dbTran,
                id: ref stringId,
                parentId: 0,
                orderPositionId: orderPositionId,
                companyId: companyId,
                anonymousCompanyName: null,
                businessUnitId: _orderBusinessUnitId,
                date: null,
                vacancyName: advString.VacancyName,
                vacancyAdditional: advString.VacancyAdditional,
                vacancyCode: null,
                responsibility: advString.Responsibility,
                requirement: advString.Requirements.Value,
                ageFrom: advString.Requirements.Age.From,
                ageTo: advString.Requirements.Age.To,
                genderId: advString.Requirements.GenderId,
                educationId: advString.Requirements.EducationId,
                experienceId: advString.Requirements.Experience.Id,
                experienceValue: advString.Requirements.Experience.Value,
                citizenshipId: advString.Requirements.CitizenshipId,
                condition: advString.Conditions.Value,
                salaryFrom: advString.Conditions.Salary.From,
                salaryTo: advString.Conditions.Salary.To,
                salaryDescription: advString.Conditions.Salary.Description,
                currencyId: advString.Conditions.Salary.CurrencyId,
                workGraphicId: advString.Conditions.WorkGraphic.Id,
                workGraphicComm: advString.Conditions.WorkGraphic.Comment,
                keywords: null,
                logo: logoBytes,
                logoFileName: advString.Logo.FileName,
                contactFirstName: advString.Contact.FirstName,
                contactSecondName: advString.Contact.SecondName,
                contactLastName: advString.Contact.LastName,
                text: null,
                isSalaryPercent: advString.Conditions.Salary.IsSalaryPercent,
                isHousing: advString.Conditions.IsHousing,
                isFood: advString.Conditions.IsFood,
                isActual: true,
                editUserId: _editUserId,
                lastEditDate: ref lastEditDate);

            return stringId;
        }

        private void CreateFullString(int companyId, int orderPositionId, AdvString advString, DbTransaction dbTran)
        {
            var stringId = CreateString(companyId, orderPositionId, advString, dbTran);

            CreateStringAddresses(stringId, companyId, advString.Addresses, dbTran);
            CreateStringOccurrences(stringId, advString.Occurrences, dbTran);
            CreateStringPhones(stringId, companyId, advString.Phones, dbTran);
            CreateStringWebs(stringId, companyId, advString.Emails, dbTran);
        }

        #endregion

        #region Update

        private bool NeedUpdateString(StringPosition stringPosition, AdvString advString)
        {
            return
                stringPosition.Date != advString.Date ||
                stringPosition.VacancyName != advString.VacancyName ||
                stringPosition.VacancyAdditional != advString.VacancyAdditional ||
                stringPosition.Requirement != advString.Requirements.Value ||
                stringPosition.AgeFrom != advString.Requirements.Age.From ||
                stringPosition.AgeTo != advString.Requirements.Age.To ||
                stringPosition.GenderId != advString.Requirements.GenderId ||
                stringPosition.EducationId != advString.Requirements.EducationId ||
                stringPosition.CitizenshipId != advString.Requirements.CitizenshipId ||
                stringPosition.ExperienceId != advString.Requirements.Experience.Id ||
                stringPosition.ExperienceValue != advString.Requirements.Experience.Value ||
                stringPosition.Responsibility != advString.Responsibility ||
                stringPosition.Condition != advString.Conditions.Value ||
                stringPosition.WorkGraphicId != advString.Conditions.WorkGraphic.Id ||
                stringPosition.WorkGraphic != advString.Conditions.WorkGraphic.Comment ||
                stringPosition.SalaryFrom != advString.Conditions.Salary.From ||
                stringPosition.SalaryTo != advString.Conditions.Salary.To ||
                stringPosition.SalaryDescription != advString.Conditions.Salary.Description ||
                stringPosition.ContactFirstName != advString.Contact.FirstName ||
                stringPosition.ContactSecondName != advString.Contact.SecondName ||
                stringPosition.ContactLastName != advString.Contact.LastName ||
                stringPosition.Logo != GetBase64Bytes(advString.Logo.Base64String) ||
                stringPosition.LogoFileName != advString.Logo.FileName;
        }

        private void UpdateString(StringPosition stringPosition, AdvString advString, DbTransaction dbTran)
        {
            if (!NeedUpdateString(stringPosition, advString))
                return;
             
            var stringId = stringPosition.Id;

            var logoBytes = GetBase64Bytes(advString.Logo.Base64String); 

            var lastEditDate = stringPosition.BeginDate;

            _repository.SetString(
                dbTran: dbTran,
                id: ref stringId,
                parentId: 0,
                orderPositionId: stringPosition.OrderPositionId,
                companyId: stringPosition.CompanyId,
                anonymousCompanyName: stringPosition.AnonimousCompanyName,
                businessUnitId: stringPosition.BusinessUnitId,
                date: stringPosition.Date,
                vacancyName: advString.VacancyName,
                vacancyAdditional: advString.VacancyAdditional,
                vacancyCode: stringPosition.VacancyCode,
                responsibility: advString.Responsibility,
                requirement: advString.Requirements.Value,
                ageFrom: advString.Requirements.Age.From,
                ageTo: advString.Requirements.Age.To,
                genderId: advString.Requirements.GenderId,
                educationId: advString.Requirements.EducationId,
                experienceId: advString.Requirements.Experience.Id,
                experienceValue: advString.Requirements.Experience.Value,
                citizenshipId: advString.Requirements.CitizenshipId,
                condition: advString.Conditions.Value,
                salaryFrom: advString.Conditions.Salary.From,
                salaryTo: advString.Conditions.Salary.To,
                salaryDescription: advString.Conditions.Salary.Description,
                currencyId: advString.Conditions.Salary.CurrencyId,
                workGraphicId: advString.Conditions.WorkGraphic.Id,
                workGraphicComm: advString.Conditions.WorkGraphic.Comment,
                keywords: stringPosition.Keywords,
                logo: logoBytes,
                logoFileName: advString.Logo.FileName,
                contactFirstName: advString.Contact.FirstName,
                contactSecondName: advString.Contact.SecondName,
                contactLastName: advString.Contact.LastName,
                text: stringPosition.Text,
                isSalaryPercent: advString.Conditions.Salary.IsSalaryPercent,
                isHousing: advString.Conditions.IsHousing,
                isFood: advString.Conditions.IsFood,
                isActual: true,
                editUserId: _editUserId,
                lastEditDate: ref lastEditDate);

            _context.Entry(stringPosition).Reload();
        }

        private bool CanUpdateFullString(PositionIm positionIm)
        {
            return positionIm != null && positionIm.PositionImType.OrderImTypeId == 1;
        }

        private void UpdateFullString(int orderPositionId, AdvString advString, DbTransaction dbTran)
        {
            var stringPosition = _context.StringPositions
                .Include(sp => sp.Addresses)
                .Include(sp => sp.Occurrences)
                .Include(sp => sp.Phones)
                .Include(sp => sp.Webs)
                .SingleOrDefault(sp => sp.OrderPositionId == orderPositionId);

            if (stringPosition == null)
                return;

            UpdateString(stringPosition, advString, dbTran);

            UpdateStringAddresses(stringPosition.Id, stringPosition.CompanyId, stringPosition.Addresses, advString.Addresses, dbTran);
            UpdateStringOccurences(stringPosition.Id, stringPosition.Occurrences, advString.Occurrences, dbTran);
            UpdateStringPhones(stringPosition.Id, stringPosition.CompanyId, stringPosition.Phones, advString.Phones, dbTran);
            UpdateStringWebs(stringPosition.Id, stringPosition.CompanyId, stringPosition.Webs, advString.Emails, dbTran);
        }

        #endregion

        #region Delete

        private bool CanDeleteFullString(PositionIm positionIm)
        {
            return positionIm.PositionImType.OrderImType.Id == 1;
        }

        private void DeleteString(StringPosition stringPosition, DbTransaction dbTran)
        {
            var stringId = stringPosition.Id;
            var lastEditDate = stringPosition.BeginDate;

            _repository.SetString(
                dbTran: dbTran,
                id: ref stringId,
                parentId: 0,
                orderPositionId: stringPosition.OrderPositionId,
                companyId: stringPosition.CompanyId,
                anonymousCompanyName: stringPosition.AnonimousCompanyName,
                businessUnitId: stringPosition.BusinessUnitId,
                date: stringPosition.Date,
                vacancyName: stringPosition.VacancyName,
                vacancyAdditional: stringPosition.VacancyAdditional,
                vacancyCode: stringPosition.VacancyCode,
                responsibility: stringPosition.Responsibility,
                requirement: stringPosition.Requirement,
                ageFrom: stringPosition.AgeFrom,
                ageTo: stringPosition.AgeTo,
                genderId: stringPosition.GenderId,
                educationId: stringPosition.EducationId,
                experienceId: stringPosition.ExperienceId,
                experienceValue: stringPosition.ExperienceValue,
                citizenshipId: stringPosition.CitizenshipId,
                condition: stringPosition.Condition,
                salaryFrom: stringPosition.SalaryFrom,
                salaryTo: stringPosition.SalaryTo,
                salaryDescription: stringPosition.SalaryDescription,
                currencyId: stringPosition.CurrencyId,
                workGraphicId: stringPosition.WorkGraphicId,
                workGraphicComm: stringPosition.WorkGraphic,
                keywords: stringPosition.Keywords,
                logo: stringPosition.Logo,
                logoFileName: stringPosition.LogoFileName,
                contactFirstName: stringPosition.ContactFirstName,
                contactSecondName: stringPosition.ContactSecondName,
                contactLastName: stringPosition.ContactLastName,
                text: stringPosition.Text,
                isSalaryPercent: stringPosition.IsSalaryPercent,
                isHousing: stringPosition.IsHousing,
                isFood: stringPosition.IsFood,
                isActual: false,
                editUserId: _editUserId,
                lastEditDate: ref lastEditDate);

            _context.Entry(stringPosition).Reload();
        }

        private void DeleteFullString(int orderPositionId, DbTransaction dbTran)
        {
            var stringPosition = _context.StringPositions
                .Include(sp => sp.Addresses)
                .Include(sp => sp.Occurrences)
                .Include(sp => sp.Phones)
                .Include(sp => sp.Webs)
                .SingleOrDefault(sp => sp.OrderPositionId == orderPositionId);

            if (stringPosition == null)
                return;

            DeleteStringAddresses(stringPosition.Addresses, dbTran);
            DeleteStringOccurrences(stringPosition.Occurrences, dbTran);
            DeleteStringPhones(stringPosition.Phones, dbTran);
            DeleteStringWebs(stringPosition.Webs, dbTran);

            DeleteString(stringPosition, dbTran);
        }

        #endregion
    }
}
