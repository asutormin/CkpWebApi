using CkpModel.Input.String.Conditions;
using CkpModel.Input.String.Requirements;
using System;
using System.Collections.Generic;

namespace CkpModel.Input.String
{
    public class StringData
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string AnonymousCompanyName { get; set; }
        public string VacancyName { get; set; }
        public string VacancyAdditional { get; set; }
        public RequirementsData RequirementsData { get; set; }
        public string Responsibility { get; set; }
        public ConditionsData ConditionsData { get; set; }
        public ContactData ContactData { get; set; }
        public LogoData LogoData { get; set; }

        public IEnumerable<PhoneData> PhonesData { get; set; }
        public IEnumerable<EmailData> EmailsData { get; set; }
        public IEnumerable<AddressData> AddressesData { get; set; }
        public IEnumerable<OccurrenceData> OccurrencesData { get; set; }

    }
}
