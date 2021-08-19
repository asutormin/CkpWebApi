using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CkpModel.Input
{
    public class OrderPositionDataLight
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
        public FormatData FormatData { get; set; }

        [Required]
        public int PriceId { get; set; }
                
        public RubricData RubricData { get; set; }

        public List<GraphicData> GraphicsData { get; set; }
    }
}
