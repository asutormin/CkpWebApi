using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.PricePosition
{
    [Table("vwa_price_positions_ex")]
    public class PricePositionEx
    {
        [Key]
        [Column("price_position_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("order_by", TypeName = "int")]
        public int OrderBy { get; set; }
    }
}
