using System;
using System.ComponentModel.DataAnnotations;

namespace CkpModel.Input
{
    public class FormatData
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
