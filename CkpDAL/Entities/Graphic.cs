using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_graphics")]
    public class Graphic
    {
        [Key]
        [Column("graphic_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("company_id", TypeName = "int")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [Column("price_position_type_id", TypeName = "int")]
        public int PricePositionTypeId { get; set; }

        [ForeignKey("PricePositionTypeId")]
        public PricePositionType PricePositionType { get; set; }

        [Column("number", TypeName = "varchar(10)")]
        public string Number { get; set; }

        [Column("deliver_date", TypeName = "datetime")]
        public DateTime DeliverDate { get; set; }

        [Column("closing_date", TypeName = "datetime")]
        public DateTime ClosingDate { get; set; }

        [Column("out_date", TypeName = "datetime")]
        public DateTime OutDate { get; set; }

        [Column("finish_date", TypeName = "datetime")]
        public DateTime FinishDate { get; set; }

        [Column("description", TypeName = "varchar(150)")]
        public string Description { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
    }
}
