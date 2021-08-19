using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CkpModel.Input
{
    public class GraphicData
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public List<int> Childs { get; set; }
    }
}
