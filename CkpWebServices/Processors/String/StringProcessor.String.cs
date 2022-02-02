using CkpDAL.Entities.String;
using CkpModel.Input.String;
using CkpServices.Helpers.Converters;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors.String
{
    partial class StringProcessor
    {
        #region Get

        public StringPosition GetStringPosition(int orderPositionId)
        {
            var stringPosition = _context.StringPositions
                .Include(sp => sp.OrderPosition)
                .Include(sp => sp.Addresses.Where(a => a.IsActual))
                .Include(sp => sp.Occurrences)
                .Include(sp => sp.Phones.Where(p => p.IsActual))
                .Include(sp => sp.Webs.Where(w => w.IsActual))
                .SingleOrDefault(sp => sp.OrderPositionId == orderPositionId);

            return stringPosition;
        }

        #endregion

        #region Create

        private StringPosition CreateStringPosition(int businessUnitId, int companyId, int orderPositionId, StringData stringData, DbTransaction dbTran)
        {
            var stringPosition = _stringFactory.Create(businessUnitId, companyId, orderPositionId, stringData);
            stringPosition = _repository.SetString(stringPosition, isActual: true, dbTran);

            return stringPosition;
        }

        #endregion

        #region Update

        private bool NeedUpdateStringPosition(StringPosition stringPosition, StringData stringData)
        {
            if (stringPosition.Date != stringData.Date ||
                stringPosition.AnonymousCompanyName != stringData.AnonymousCompanyName ||
                stringPosition.VacancyName != stringData.VacancyName ||
                stringPosition.VacancyAdditional != stringData.VacancyAdditional ||
                stringPosition.Requirement != stringData.RequirementsData.Value ||
                stringPosition.AgeFrom != stringData.RequirementsData.AgeData.From ||
                stringPosition.AgeTo != stringData.RequirementsData.AgeData.To ||
                stringPosition.GenderId != stringData.RequirementsData.GenderId ||
                stringPosition.EducationId != stringData.RequirementsData.EducationId ||
                stringPosition.CitizenshipId != stringData.RequirementsData.CitizenshipId ||
                stringPosition.ExperienceId != stringData.RequirementsData.ExperienceData.Id ||
                stringPosition.ExperienceValue != stringData.RequirementsData.ExperienceData.Value ||
                stringPosition.Responsibility != stringData.Responsibility ||
                stringPosition.Condition != stringData.ConditionsData.Value ||
                stringPosition.WorkGraphicId != stringData.ConditionsData.WorkGraphicData.Id ||
                stringPosition.WorkGraphic != stringData.ConditionsData.WorkGraphicData.Comment ||
                stringPosition.SalaryFrom != stringData.ConditionsData.SalaryData.From ||
                stringPosition.SalaryTo != stringData.ConditionsData.SalaryData.To ||
                stringPosition.SalaryDescription != stringData.ConditionsData.SalaryData.Description ||
                stringPosition.IsSalaryPercent != stringData.ConditionsData.SalaryData.IsSalaryPercent ||
                stringPosition.ContactFirstName != stringData.ContactData.FirstName ||
                stringPosition.IsFood != stringData.ConditionsData.IsFood ||
                stringPosition.IsHousing != stringData.ConditionsData.IsHousing ||
                stringPosition.ContactSecondName != stringData.ContactData.SecondName ||
                stringPosition.ContactLastName != stringData.ContactData.LastName ||
                stringPosition.Logo != Base64ToBytesConverter.Convert(stringData.LogoData.Base64String) ||
                stringPosition.LogoFileName != stringData.LogoData.FileName)
                return true;

            return false;
        }

        public StringPosition UpdateString(StringPosition stringPosition, StringData stringData, DbTransaction dbTran)
        {
            if (!NeedUpdateStringPosition(stringPosition, stringData))
                return stringPosition;

            stringPosition.AnonymousCompanyName = stringData.AnonymousCompanyName;
            stringPosition.VacancyName = stringData.VacancyName;
            stringPosition.VacancyAdditional = stringData.VacancyAdditional;
            stringPosition.VacancyCode = stringPosition.VacancyCode;
            stringPosition.Responsibility = stringData.Responsibility;
            stringPosition.Requirement = stringData.RequirementsData.Value;
            stringPosition.AgeFrom = stringData.RequirementsData.AgeData.From;
            stringPosition.AgeTo = stringData.RequirementsData.AgeData.To;
            stringPosition.GenderId = stringData.RequirementsData.GenderId;
            stringPosition.EducationId = stringData.RequirementsData.EducationId;
            stringPosition.ExperienceId = stringData.RequirementsData.ExperienceData.Id;
            stringPosition.ExperienceValue = stringData.RequirementsData.ExperienceData.Value;
            stringPosition.CitizenshipId = stringData.RequirementsData.CitizenshipId;
            stringPosition.Condition = stringData.ConditionsData.Value;
            stringPosition.SalaryFrom = stringData.ConditionsData.SalaryData.From;
            stringPosition.SalaryTo = stringData.ConditionsData.SalaryData.To;
            stringPosition.SalaryDescription = stringData.ConditionsData.SalaryData.Description;
            stringPosition.CurrencyId = stringData.ConditionsData.SalaryData.CurrencyId;
            stringPosition.WorkGraphicId = stringData.ConditionsData.WorkGraphicData.Id;
            stringPosition.WorkGraphic = stringData.ConditionsData.WorkGraphicData.Comment;
            stringPosition.Keywords = stringPosition.Keywords;
            stringPosition.Logo = Base64ToBytesConverter.Convert(stringData.LogoData.Base64String);
            stringPosition.LogoFileName = stringData.LogoData.FileName;
            stringPosition.ContactFirstName = stringData.ContactData.FirstName;
            stringPosition.ContactSecondName = stringData.ContactData.SecondName;
            stringPosition.ContactLastName = stringData.ContactData.LastName;
            stringPosition.Text = stringPosition.Text;
            stringPosition.IsSalaryPercent = stringData.ConditionsData.SalaryData.IsSalaryPercent;
            stringPosition.IsHousing = stringData.ConditionsData.IsHousing;
            stringPosition.IsFood = stringData.ConditionsData.IsFood;

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
