using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_business_unit")]
    public class BusinessUnit
    {
        [Key]
        [Column("business_unit_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Column("short_name", TypeName = "varchar(10)")]
        public string ShortName { get; set; }

        [Column("legal_person_id", TypeName = "int")]
        public int LegalPersonId { get; set; }

        [Column("cash_id", TypeName = "int")]
        public int CashId { get; set; }

        [ForeignKey("CashId")]
        public Cash Cash { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

        [Column("accounts_with_nds", TypeName = "bit")]
        public bool AccountsWithNds { get; set; }
    }
}
