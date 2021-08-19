using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_package_positions")]
    public class PackagePosition
    {
        [Key]
        [Column("package_position_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("package_id", TypeName = "int")]
        public int PricePositionId { get; set; }

        [ForeignKey("PricePositionId")]
        public PricePositionActual PricePosition { get; set; }

        [Column("price_id", TypeName = "int")]
        public int PriceId { get; set; }

        [ForeignKey("PriceId")]
        public Price Price { get; set; }
    }
}
