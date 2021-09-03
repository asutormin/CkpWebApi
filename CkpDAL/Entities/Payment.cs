using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
	[Table("vwa_payments")]
	public class Payment
    {
		[Key]
		[Column("payment_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("number", TypeName = "varchar(50)")]
		public string Number { get; set; }

		[Column("accounts_number", TypeName = "varchar(100)")]
		public string AccountsNumber { get; set; }

		[Column("orders_number", TypeName = "varchar(100)")]
		public string OrdersNumber { get; set; }

		[Required]
		[Column("is_distribution", TypeName = "bit")]
		public bool IsDistribution { get; set; }

		[Required]
		[Column("payment_type_id", TypeName = "int")]
		public int PaymentTypeId { get; set; }

		[ForeignKey("PaymentTypeId")]
		public PaymentType PaymentType { get; set; }

		[Required]
		[Column("company_id", TypeName = "int")]
		public int CompanyId { get; set; }

		[Required]
		[Column("business_unit_id", TypeName = "int")]
		public int BusinessUnitId { get; set; }

		[ForeignKey("BusinessUnitId")]
		public BusinessUnit BusinessUnit { get; set; }

		[Required]
		[Column("legal_person_id", TypeName = "int")]
		public int LegalPersonId { get; set; }

		[Required]
		[Column("cash_id", TypeName = "int")]
		public int CashId { get; set; }

		[Required]
		[Column("summ", TypeName = "money")]
		public float Summ { get; set; }

		[Required]
		[Column("undisposed_summ", TypeName = "money")]
		public float UndisposedSum { get; set; }

		[Required]
		[Column("payment_date", TypeName = "datetime")]
		public DateTime PaymentDate { get; set; }

		[Column("employee_id", TypeName = "int")]
		public int? EmployeeId { get; set; }

		[Required]
		[Column("payment_foundation", TypeName = "varchar(50)")]
		public string PaymentFoundation { get; set; }

		[Column("description", TypeName = "varchar(300)")]
		public string Description { get; set; }

		[Required]
		[Column("payment_agent_id", TypeName = "int")]
		public int PaymentAgentId { get; set; }

		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }
	}
}
