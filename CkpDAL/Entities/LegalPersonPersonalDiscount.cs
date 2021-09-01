using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_legal_person_personal_discounts")]
    public class LegalPersonPersonalDiscount
    {
        [Key]
        [Column("legal_person_personal_discount_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("legal_person_id", TypeName = "int")]
        public int LegalPersonId { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }
        
        [ForeignKey("BusinessUnitId")]
        public BusinessUnit BusinessUnit { get; set; }

        [Column("supplier_id", TypeName = "int")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [Column("price_position_type_id", TypeName = "int")]
        public int PricePositionTypeId { get; set; }

        [ForeignKey("PricePositionTypeId")]
        public PricePositionType PricePositionType { get; set; }

        [Column("discount_percent", TypeName = "int")]
        public int DiscountPercent { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
    }
}
