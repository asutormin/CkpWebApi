using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_order_ims")]
    public class OrderIm
    {
		[Key]
		[Required]
        [Column("order_id", TypeName = "int")]
        public int OrderId { get; set; }

		[ForeignKey("OrderId")]
		public Order Order { get; set; }

		[Key]
		[Required]
		[Column("order_im_type_id", TypeName = "int")]
		public int OrderImTypeId { get; set; }

		[ForeignKey("OrderImTypeId")]
		public OrderImType OrderImType { get; set; }

		[Required]
		[Column("maket_status_id", TypeName = "int")]
		public int MaketStatusId { get; set; }

		[Required]
		[Column("maket_category_id", TypeName = "int")]
		public int MaketCategoryId { get; set; }

		[Required]
		[Column("brief", TypeName = "varchar(3000)")]
		public string Brief { get; set; }

		[Required]
		[Column("visa", TypeName = "bit")]
		public bool NeedVisa { get; set; }

		[Required]
		[Column("need_verify", TypeName = "bit")]
		public bool NeedVerify { get; set; }

		[Column("designer_id", TypeName = "int")]
		public int? DesignerId { get; set; }

		[Column("comments", TypeName = "varchar(300)")]
		public string Comments { get; set; }

		[Column("max_closing_date", TypeName = "datetime")]
		public DateTime? MaxClosingDate { get; set; }

		[Required]
		[Column("viewed", TypeName = "bit")]
		public bool IsViewed { get; set; }

		[Required]
		[Column("im_replace_status_id", TypeName = "int")]
		public int ImReplaceStatusId { get; set; }

		[Required]
		[Column("order_im_begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }
	}
}
