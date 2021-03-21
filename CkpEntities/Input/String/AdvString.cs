using CkpEntities.Input.String.Conditions;
using CkpEntities.Input.String.Requirements;
using System;
using System.Collections.Generic;

namespace CkpEntities.Input.String
{
    public class AdvString
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string VacancyName { get; set; }
        public string VacancyAdditional { get; set; }
        public AdvRequirements Requirements { get; set; }
        public string Responsibility { get; set; }
        public AdvConditions Conditions { get; set; }
        public AdvContact Contact { get; set; }
        public AdvLogo Logo { get; set; }

        public IEnumerable<AdvPhone> Phones { get; set; }
        public IEnumerable<AdvEmail> Emails { get; set; }
        public IEnumerable<AdvAddress> Addresses { get; set; }
        public IEnumerable<AdvOccurrence> Occurrences { get; set; }

    }
}
