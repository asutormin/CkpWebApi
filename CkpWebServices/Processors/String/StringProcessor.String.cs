using CkpDAL.Model.String;
using CkpEntities.Input.String;
using CkpServices.Helpers.Converters;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.String
{
    partial class StringProcessor
    {
        #region Get

        public StringPosition GetString(int orderPositionId)
        {
            var stringPosition = _context.StringPositions
            .Include(sp => sp.Addresses)
            .Include(sp => sp.Occurrences)
            .Include(sp => sp.Phones)
            .Include(sp => sp.Webs)
            .SingleOrDefault(sp => sp.OrderPositionId == orderPositionId);

            return stringPosition;
        }

        #endregion

        #region Create

        private StringPosition CreateStringPosition(int businessUnitId, int companyId, int orderPositionId, AdvString advString, DbTransaction dbTran)
        {
            var stringPosition = _stringFactory.Create(businessUnitId, companyId, orderPositionId, advString);
            stringPosition = _repository.SetString(stringPosition, isActual: true, dbTran);

            return stringPosition;
        }

        #endregion

        #region Update

        private bool NeedUpdateStringPosition(StringPosition stringPosition, AdvString advString)
        {
            if (stringPosition.Date != advString.Date ||
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
                stringPosition.Logo != Base64ToBytesConverter.Convert(advString.Logo.Base64String) ||
                stringPosition.LogoFileName != advString.Logo.FileName)
                return true;

            return false;
        }

        public StringPosition UpdateString(StringPosition stringPosition, AdvString advString, DbTransaction dbTran)
        {
            if (!NeedUpdateStringPosition(stringPosition, advString))
                return stringPosition;

            stringPosition.VacancyName = advString.VacancyName;
            stringPosition.VacancyAdditional = advString.VacancyAdditional;
            stringPosition.VacancyCode = stringPosition.VacancyCode;
            stringPosition.Responsibility = advString.Responsibility;
            stringPosition.Requirement = advString.Requirements.Value;
            stringPosition.AgeFrom = advString.Requirements.Age.From;
            stringPosition.AgeTo = advString.Requirements.Age.To;
            stringPosition.GenderId = advString.Requirements.GenderId;
            stringPosition.EducationId = advString.Requirements.EducationId;
            stringPosition.ExperienceId = advString.Requirements.Experience.Id;
            stringPosition.ExperienceValue = advString.Requirements.Experience.Value;
            stringPosition.CitizenshipId = advString.Requirements.CitizenshipId;
            stringPosition.Condition = advString.Conditions.Value;
            stringPosition.SalaryFrom = advString.Conditions.Salary.From;
            stringPosition.SalaryTo = advString.Conditions.Salary.To;
            stringPosition.SalaryDescription = advString.Conditions.Salary.Description;
            stringPosition.CurrencyId = advString.Conditions.Salary.CurrencyId;
            stringPosition.WorkGraphicId = advString.Conditions.WorkGraphic.Id;
            stringPosition.WorkGraphic = advString.Conditions.WorkGraphic.Comment;
            stringPosition.Keywords = stringPosition.Keywords;
            stringPosition.Logo = Base64ToBytesConverter.Convert(advString.Logo.Base64String);
            stringPosition.LogoFileName = advString.Logo.FileName;
            stringPosition.ContactFirstName = advString.Contact.FirstName;
            stringPosition.ContactSecondName = advString.Contact.SecondName;
            stringPosition.ContactLastName = advString.Contact.LastName;
            stringPosition.Text = stringPosition.Text;
            stringPosition.IsSalaryPercent = advString.Conditions.Salary.IsSalaryPercent;
            stringPosition.IsHousing = advString.Conditions.IsHousing;
            stringPosition.IsFood = advString.Conditions.IsFood;

            stringPosition = _repository.SetString(stringPosition, isActual: true, dbTran);

            return stringPosition;
        }

        #endregion

        #region Delete

        public void DeleteString(StringPosition stringPosition, DbTransaction dbTran)
        {
            _repository.SetString(stringPosition, isActual: false, dbTran);
            _context.Entry(stringPosition).Reload();
        }

        #endregion
    }
}
