using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_prices")]
    public class Price
    {
        [Key]
        [Column("price_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }

        [Column("price_position_id", TypeName = "int")]
        public int PricePositionId { get; set; }

        [ForeignKey("PricePositionId")]
        public PricePositionActual PricePosition { get; set; }

        [Column("price_value", TypeName = "money")]
        public float Value { get; set; }

        [Column("markup", TypeName = "float")]
        public float Markup { get; set; }

        [Column("discount", TypeName = "float")]
        public float Discount { get; set; }

        [Column("price_permissions", TypeName = "bigint")]
        public long PermissionFlag { get; set; }

        [Column("actual_begin", TypeName = "datetime")]
        public DateTime ActualBegin { get; set; }

        [Column("actual_end", TypeName = "datetime")]
        public DateTime ActualEnd { get; set; }

        [Column("price_begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        public virtual SupplierProjectRelation SupplierProjectRelation { get; set; }

        public virtual ICollection<PackagePosition> PackagePositions { get; set; }
    }
}
