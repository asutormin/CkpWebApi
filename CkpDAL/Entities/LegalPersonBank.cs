using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_legal_person_banks")]
    public class LegalPersonBank
    {
		[Key]
		[Required]
		[Column("legal_persons_banks_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("legal_person_id", TypeName = "int")]
		public int LegalPersonId { get; set; }

		[ForeignKey("LegalPersonId")]
		public LegalPerson LegalPerson { get; set; }

		[Required]
		[Column("bank_id", TypeName = "int")]
		public int BankId { get; set; }

		[ForeignKey("BankId")]
		public Bank Bank { get; set; }

		[Required]
		[Column("settlement_account", TypeName = "varchar(20)")]
		public string SettlementAccount { get; set; }

		[Required]
		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }
	}
}
