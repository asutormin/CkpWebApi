using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_legal_person_price_position_type_discounts")]
    public class LegalPersonPricePositionTypeDiscount
    {
        [Key]
        [Column("legal_person_price_position_type_discount_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("legal_person_id", TypeName = "int")]
        public int LegalPersonId { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

        [Column("price_position_type_id", TypeName = "int")]
        public int PricePositionTypeId { get; set; }

        [Column("discount_percent", TypeName = "int")]
        public int DiscountPercent { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
    }
}
