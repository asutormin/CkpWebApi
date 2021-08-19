using System;
using System.ComponentModel.DataAnnotations;

namespace CkpModel.Input
{
    public class RubricData
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public DateTime Version { get; set; }
    }
}
