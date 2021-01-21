using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("t_order_im_types")]
    public class OrderImType
    {
        [Key]
        [Required]
        [Column("order_im_type_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("order_im_type_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }

        public virtual ICollection<PositionImType> PositionImTypes { get; set; }
    }
}
