using CkpDAL;
using CkpDAL.Entities.String;
using CkpDAL.Repository;
using CkpInfrastructure.Configuration;
using CkpModel.Output.String;
using CkpServices.Interfaces;
using CkpServices.Processors;
using CkpServices.Processors.Interfaces;
using CkpServices.Processors.String;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CkpServices
{
    public class StringService : IStringService
    {
        private readonly BPFinanceContext _context;
        private readonly IStringProcessor _stringProcessor;
        private readonly IHandbookProcessor _handbookProcessor;

        public StringService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            _stringProcessor = new StringProcessor(
                _context,
                repository);

            _handbookProcessor = new HandbookProcessor(
                _context,
                repository);
        }

        public async Task<List<AddressInfo>> GetAddressesAsync(int clientLegalPersonId, string description)
        {
            return await _stringProcessor.GetAddressesAsync(clientLegalPersonId, description);
        }

        public StringPositionInfo GetStringPosition(int orderPositionId)
        {
            var stringPosition = _stringProcessor.GetStringPosition(orderPositionId);

            var stringPositionInfo = new StringPositionInfo();

            stringPositionInfo.Name = stringPosition.VacancyName;

            stringPositionInfo.Company = IsRrd(stringPosition.OrderPosition.SupplierId)
                ? stringPosition.AnonymousCompanyName
                : stringPosition.VacancyAdditional;

            var requirementsBuilder = new StringBuilder();

            var requirements = stringPosition.Requirement;
            if (!string.IsNullOrEmpty(requirements))
            {
                requirements = RemoveLastDot(stringPosition.Requirement);
                requirementsBuilder.Append(string.Format(" {0}.", requirements));
            }

            // Для изданий не РРД
            if (!IsRrd(stringPosition.OrderPosition.SupplierId))
            {
                if (stringPosition.EducationId.HasValue)
                {
                    var education = _handbookProcessor.GetEducation(stringPosition.EducationId.Value);
                    education = RemoveLastDot(education);
                    requirementsBuilder.Append(string.Format(" Образование: {0}.", education));
                }

                if (stringPosition.ExperienceId.HasValue)
                {
                    var experience = _handbookProcessor.GetExperience(stringPosition.ExperienceId.Value);
                    experience = RemoveLastDot(experience);
                    requirementsBuilder.Append(string.Format(" Опыт работы: {0}", experience));

                    var experienceValue = stringPosition.ExperienceValue;
                    if (experienceValue != 0)
                        requirementsBuilder.Append(string.Format(" {0} лет", experienceValue));

                    requirementsBuilder.Append(".");
                }
            }

            stringPositionInfo.Requirements = requirementsBuilder.ToString();

            var responsibilities = stringPosition.Responsibility;
            if (!string.IsNullOrEmpty(responsibilities))
            {
                responsibilities = RemoveLastDot(responsibilities);
                stringPositionInfo.Responsibilities = string.Format("{0}.", responsibilities);
            }

            var salaryBuilder = new StringBuilder();

            if (!IsRrd(stringPosition.OrderPosition.SupplierId))
            {
                if (stringPosition.SalaryFrom.HasValue && stringPosition.SalaryTo.HasValue)
                {
                    if (stringPosition.SalaryFrom.Value == stringPosition.SalaryTo.Value)
                        salaryBuilder.Append(stringPosition.SalaryFrom.Value.ToString());
                    else
                        salaryBuilder.Append(string.Format(
                            "от {0} до {1}", stringPosition.SalaryFrom.Value, stringPosition.SalaryTo.Value));
                }
                else
                {
                    if (stringPosition.SalaryFrom.HasValue)
                        salaryBuilder.Append(string.Format("от {0}", stringPosition.SalaryFrom.Value));
                    else if (stringPosition.SalaryTo.HasValue)
                        salaryBuilder.Append(string.Format("до {0}", stringPosition.SalaryTo.Value));
                    else
                        salaryBuilder.Append("договорная");
                }

                if (stringPosition.CurrencyId.HasValue)
                {
                    var currency = _handbookProcessor.GetCurrency(stringPosition.CurrencyId.Value);
                    salaryBuilder.Append(string.Format(" {0}", currency));
                }

                if (stringPosition.IsSalaryPercent)
                    salaryBuilder.Append(" +%");
            }

            stringPositionInfo.Salary = salaryBuilder.ToString();

            var conditionsBuilder = new StringBuilder();

            var condition = stringPosition.Condition;
            if (!string.IsNullOrEmpty(condition))
            {
                condition = RemoveLastDot(condition);
                conditionsBuilder.Append(string.Format("{0}.", condition));
            }

            if (!IsRrd(stringPosition.OrderPosition.SupplierId))
            {
                if (stringPosition.WorkGraphicId.HasValue)
                {
                    var workGraphic = _handbookProcessor.GetWorkGraphic(stringPosition.WorkGraphicId.Value);
                    conditionsBuilder.Append(string.Format(" График работы: {0}", workGraphic));

                    var workGraphicDescription = stringPosition.WorkGraphic;
                    if (!string.IsNullOrEmpty(workGraphicDescription))
                    {
                        workGraphicDescription = RemoveLastDot(workGraphicDescription);
                        conditionsBuilder.Append(string.Format(" {0}", workGraphicDescription));
                    }

                    conditionsBuilder.Append(".");
                }

                if (stringPosition.IsFood)
                    stringPositionInfo.IsFood = stringPosition.IsFood;

                if (stringPosition.IsHousing)
                    stringPositionInfo.IsHousing = stringPosition.IsHousing;
            }

            var conditions = conditionsBuilder.ToString();
            if (!string.IsNullOrEmpty(conditions))
                stringPositionInfo.Conditions = conditionsBuilder.ToString();

            var occurrences = string.Join(", ", GetOccurrences(stringPosition.Occurrences.ToList()));
            if (!string.IsNullOrEmpty(occurrences))
                stringPositionInfo.Occurrences = occurrences;

            var address = stringPosition.Addresses.FirstOrDefault();
            if (address != null)
                stringPositionInfo.Address = " " + address.Description;

            if (!IsRrd(stringPosition.OrderPosition.SupplierId))
            {
                var contactBuilder = new StringBuilder();

                var firstName = stringPosition.ContactFirstName;
                if (!string.IsNullOrEmpty(firstName))
                    contactBuilder.Append(firstName);

                var secondName = stringPosition.ContactSecondName;
                if (!string.IsNullOrEmpty(secondName))
                    contactBuilder.Append(string.Format(" {0}", secondName));

                var lastName = stringPosition.ContactLastName;
                if (!string.IsNullOrEmpty(lastName))
                    contactBuilder.Append(string.Format(" {0}", lastName));

                stringPositionInfo.ContactPerson = contactBuilder.ToString();
            }

            var phones = " " + string.Join(", ", GetPhones(stringPosition.Phones.ToList()));
            if (!string.IsNullOrEmpty(phones))
                stringPositionInfo.Phones = RemoveLastDot(phones);

            var emails = " " + string.Join(", ", GetEmails(stringPosition.Webs.ToList()));
            if (!string.IsNullOrEmpty(emails))
                stringPositionInfo.Email = emails;

            return stringPositionInfo;
        }

        private List<string> GetOccurrences(List<StringOccurrence> stringOccurrences)
        {
            var occurrenceNames = new List<string>();

            foreach (var stringOccurrence in stringOccurrences)
            {
                if (stringOccurrence.TypeId == 10)
                {
                    var occurrence = _context.Cities.SingleOrDefault(ct => ct.Id == stringOccurrence.OccurrenceId);

                    if (occurrence != null)
                        occurrenceNames.Add(occurrence.Name);
                }

                if (stringOccurrence.TypeId == 11)
                {
                    var occurrence = _context.Metros.SingleOrDefault(m => m.Id == stringOccurrence.OccurrenceId);

                    if (occurrence != null)
                        occurrenceNames.Add(occurrence.Name);
                }
            }

            return occurrenceNames;
        }

        private List<string> GetPhones(List<StringPhone> stringPhones)
        {
            var phones = new List<string>();
            var sortedStringPhones = stringPhones.OrderBy(p => p.OrderBy);

            foreach (var stringPhone in sortedStringPhones)
            {
                if (!stringPhone.IsActual)
                    continue;

                var phoneBuilder = new StringBuilder();

                phoneBuilder.Append(string.Format("{0} ({1}) {2}", stringPhone.CountryCode, stringPhone.Code, stringPhone.Number));

                var additionalNumber = stringPhone.AdditionalNumber;
                if (!string.IsNullOrEmpty(additionalNumber))
                    phoneBuilder.Append(string.Format(" доб. {0}", additionalNumber));

                var description = stringPhone.Description;
                if (!string.IsNullOrEmpty(description))
                    phoneBuilder.Append(string.Format(" {0}", description));

                phones.Add(phoneBuilder.ToString());
            }

            return phones;
        }

        private List<string> GetEmails(List<StringWeb> stringWebs)
        {
            var emails = new List<string>();

            foreach (var stringWeb in stringWebs)
            {
                if (stringWeb.IsActual)
                    continue;

                var email = stringWeb.Description;
                emails.Add(email);
            }

            return emails;
        }

        private string RemoveLastDot(string value)
        {
            return value.EndsWith(".")
                ? value.Remove(value.Length - 1)
                : value;
        }

        private bool IsRrd(int supplierId)
        {
            var rrdIds = new List<int> { 4671, 5823, 6212 };
            return rrdIds.Contains(supplierId);
        }
    }
}
