using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_legal_persons")]
    public class LegalPerson
    {
        [Key]
        [Column("legal_person_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("legal_person_name", TypeName = "varchar(250)")]
        public string Name { get; set; }

        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [Column("inn", TypeName = "varchar(12)")]
        public string Inn { get; set; }

        [Column("kpp", TypeName = "varchar(9)")]
        public string Kpp { get; set; }

        [Column("okpo", TypeName = "varchar(8)")]
        public string Okpo { get; set; }

        [Column("legal_address", TypeName = "varchar(150)")]
        public string LegalAddress { get; set; }

        [Column("discount_percent", TypeName = "int")]
        public int DiscountPercent { get; set; }

        public AccountSettings AccountSettings { get; set; }

        public LoginSettings LoginSettings { get; set; }

        public virtual ICollection<LegalPersonBank> LegalPersonBanks { get; set; }

        public virtual ICollection<LegalPersonSign> LegalPersonSigns { get; set; }

        public virtual ICollection<LegalPersonPersonalDiscount> PersonalDiscounts { get; set; }
    }
}
