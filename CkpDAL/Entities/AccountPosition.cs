using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_account_positions")]
    public class AccountPosition
    {
		[Key]
		[Column("account_position_id", TypeName = "int")]
		public int Id { get; set; }

		[Column("account_id", TypeName = "int")]
		public int AccountId { get; set; }

		[ForeignKey("AccountId")]
		public Account Account { get; set; }

		[Column("nomenclature", TypeName = "varchar(max)")]
		public string Nomenclature { get; set; }

		[Column("position_name", TypeName = "varchar(max)")]
		public string Name { get; set; }

		[Column("position_count", TypeName = "int")]
		public int Count { get; set; }

		[Column("position_cost", TypeName = "money")]
		public float Cost { get; set; }

		[Column("position_sum", TypeName = "money")]
		public float Sum { get; set; }

		[Column("position_discount", TypeName = "float")]
		public float Discount { get; set; }

		[Column("act_count", TypeName = "int")]
		public int ActCount { get; set; }

		[Column("first_out_date", TypeName = "datetime")]
		public DateTime? FirstOutDate { get; set; }

		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }
	}
}
