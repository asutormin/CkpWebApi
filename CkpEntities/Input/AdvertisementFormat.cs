using System;
using System.ComponentModel.DataAnnotations;

namespace CkpEntities.Input
{
    public class AdvertisementFormat
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public DateTime Version { get; set; }
        
        [Required]
        public int FormatTypeId { get; set; }

        public float? FirstSize { get; set; }

        public float? SecondSize { get; set; }
    }
}
