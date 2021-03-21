using System;
using System.ComponentModel.DataAnnotations;

namespace CkpEntities.Input
{
    public class AdvertisementRubric
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public DateTime Version { get; set; }
    }
}
