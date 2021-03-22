using CkpDAL.Model.PricePosition;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    public class PricePositionBase
    {
        [Key]
        [Column("price_position_id", TypeName = "int")]
        public int Id { get; set; }

        [ForeignKey("Id")]
        public PricePositionEx PricePositionEx { get; set; }

        [Column("price_position_name", TypeName = "varchar(150)")]
        public string Name { get; set; }

        [Column("company_id", TypeName = "int")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [Column("price_position_type_id", TypeName = "int")]
        public int PricePositionTypeId { get; set; }

        [ForeignKey("PricePositionTypeId")]
        public PricePositionType PricePositionType { get; set; }

        [Column("package_length", TypeName = "int")]
        public int PackageLength { get; set; }

        [Column("first_size", TypeName = "float")]
        public float? FirstSize { get; set; }

        [Column("second_size", TypeName = "float")]
        public float? SecondSize { get; set; }

        [Column("is_show_size", TypeName = "bit")]
        public bool IsShowSize { get; set; }

        [Column("unit_id", TypeName = "int")]
        public int? UnitId { get; set; }

        [ForeignKey("UnitId")]
        public Unit Unit { get; set; }
    }
}
