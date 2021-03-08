using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_cashes")]
    public class Cash
    {
        [Key]
        [Column("cash_id", TypeName = "int")]
        public int Id { get; set; }

		[Column("business_unit_id", TypeName = "int")]
		public int BusinessUnitId { get; set; }

		[ForeignKey("BusinessUnitId")]
		public BusinessUnit BusinessUnit { get; set; }

		[Column("cash_name", TypeName = "varchar(50)")]
		public string Name { get; set; }

		[Column("is_cashless", TypeName = "bit")]
		public bool IsCashLess { get; set; }

		[Column("legal_person_id", TypeName = "int")]
		public int LegalPersonId { get; set; }

		[ForeignKey("LegalPersonId")]
		public LegalPerson LegalPerson { get; set; }

		[Column("legal_persons_banks_id", TypeName = "int")]
		public int LegalPersonBankId { get; set; }

		[ForeignKey("LegalPersonBankId")]
		public LegalPersonBank LegalPersonBank { get; set; }

		[Column("begin_sum", TypeName = "money")]
		public float BeginSum { get; set; }

		[Column("description", TypeName = "varchar(300)")]
		public string Description { get; set; }

		[Column("order_by", TypeName = "int")]
		public int OrderBy { get; set; }

		[Column("is_hidden", TypeName = "bit")]
		public bool IsHidden { get; set; }

		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }
	}
}
