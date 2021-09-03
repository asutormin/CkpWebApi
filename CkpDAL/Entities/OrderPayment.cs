using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CkpDAL.Entities
{
	[Table("vwa_order_payments")]
	public class OrderPayment
    {
		[Key]
		[Column("order_payment_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("payment_id", TypeName = "int")]
		public int PaymentId { get; set; }

		[ForeignKey("PaymentId")]
		public Payment Payment { get; set; }

		[Required]
		[Column("order_id", TypeName = "int")]
		public int OrderId { get; set; }

		[Required]
		[Column("paid_summ", TypeName = "money")]
		public float PaidSum { get; set; }

		[Required]
		[Column("distribution_date", TypeName = "datetime")]
		public DateTime DistributionDate { get; set; }

		[Column("description", TypeName = "varchar(300)")]
		public string Description { get; set; }

		[Required]
		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }

	}
}
