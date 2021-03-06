using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_price_position_types")]
    public class PricePositionType
    {
        [Required]
        [Column("price_position_type_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("price_position_type_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Column("position_im_type_id", TypeName = "int")]
        public int? PositionImTypeId { get; set; }

        [ForeignKey("PositionImTypeId")]
        public PositionImType PositionImType { get; set; }

        [Column("is_enable_second_size", TypeName = "bit")]
        public bool EnableSecondSize { get; set; }

        public virtual ICollection<PricePositionActual> PricePositions { get; set; }

        public virtual ICollection<Graphic> Graphics { get; set; }
    }
}
