using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CkpWebApi.InputEntities
{
    public class AdvertisementLight
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int OrderPositionId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int ClientLegalPersonId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public AdvertisementFormat Format { get; set; }

        [Required]
        public int PriceId { get; set; }

        [Required]
        public AdvertisementRubric Rubric { get; set; }

        [Required]
        public List<AdvertisementGraphic> Graphics { get; set; }
    }
}
