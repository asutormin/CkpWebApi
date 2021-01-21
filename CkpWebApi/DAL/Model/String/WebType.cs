using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CkpWebApi.DAL.Model.String
{
    [Table("t_web_types")]
    public class WebType
    {
        [Key]
        [Required]
        [Column("web_type_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("web_type_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
