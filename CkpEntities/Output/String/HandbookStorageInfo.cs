using System.Collections.Generic;

namespace CkpModel.Output.String
{
    public class HandbookStorageInfo
    {
        public List<EducationInfo> Educations { get; set; }
        public List<ExperienceInfo> Experiences { get; set; }
        public List<CurrencyInfo> Currencies { get; set; }
        public List<WorkGraphicInfo> WorkGraphics { get; set; }
        public List<OccurrenceInfo> Occurrences { get; set; }
    }
}
