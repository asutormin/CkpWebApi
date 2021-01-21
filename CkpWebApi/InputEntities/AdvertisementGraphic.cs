using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CkpWebApi.InputEntities
{
    public class AdvertisementGraphic
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public List<int> Childs { get; set; }
    }
}
