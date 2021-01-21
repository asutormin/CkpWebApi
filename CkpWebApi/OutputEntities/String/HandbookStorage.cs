using System.Collections.Generic;

namespace CkpWebApi.OutputEntities.String
{
    public class HandbookStorage
    {
        public List<Education> Educations { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<WorkGraphic> WorkGraphics { get; set; }
        public List<Occurrence> Occurrences { get; set; }
    }
}
