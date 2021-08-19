using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_order_positions")]
    public class OrderPosition
    {
        [Key]
        [Column("order_position_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("order_id", TypeName = "int")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Column("parent_order_position_id", TypeName = "int")]
        public int? ParentOrderPositionId { get; set; }

        [ForeignKey("ParentOrderPositionId")]
        public virtual OrderPosition ParentOrderPosition { get; set; }

        [Column("supplier_id", TypeName = "int")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [Column("price_id", TypeName = "int")]
        public int PriceId { get; set; }

        [ForeignKey("PriceId")]
        public Price Price { get; set; }

        [Column("price_position_id", TypeName = "int")]
        public int PricePositionId { get; set; }

        [Column("price_position_version_date", TypeName = "datetime")]
        public DateTime PricePositionVersion { get; set; }

        [ForeignKey("PricePositionId, PricePositionVersion")]
        public PricePositionVersionable PricePosition { get; set; }

        [Column("markup", TypeName = "float")]
        public float Markup { get; set; }

        [Column("client_discount", TypeName = "float")]
        public float Discount { get; set; }

        [Column("compensation", TypeName = "float")]
        public float Compensation { get; set; }

        [Column("nds", TypeName = "float")]
        public float Nds { get; set; }

        [Column("rubric_id", TypeName = "int")]
        public int? RubricId { get; set; }

        [Column("rubric_version_date", TypeName = "datetime")]
        public DateTime? RubricVersion { get; set; }

        [Column("client_package_discount", TypeName = "float")]
        public float ClientPackageDiscount { get; set; }

        [Column("description", TypeName = "varchar(300)")]
        public string Description { get; set; }

        [Column("need_confirmation", TypeName = "bit")]
        public bool NeedConfirmation { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }

        public PositionIm PositionIm { get; set; }

        public virtual ICollection<OrderPosition> ChildOrderPositions { get; set; }

        public virtual ICollection<GraphicPosition> GraphicPositions { get; set; }

        public virtual ICollection<RubricPosition> RubricPositions { get; set; }
    }
}
