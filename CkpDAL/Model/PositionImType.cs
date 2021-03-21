using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("t_position_im_types")]
    public class PositionImType
    {
        [Key]
        [Required]
        [Column("position_im_type_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("position_im_type_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("order_im_type_id", TypeName = "int")]
        public int OrderImTypeId { get; set; }

        [ForeignKey("OrderImTypeId")]
        public OrderImType OrderImType { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }

        [Required]
        [Column("unload_ims", TypeName = "bit")]
        public bool UnloadIms { get; set; }

        public virtual ICollection<PricePositionType> PricePositionTypes { get; set; }
    }
}
