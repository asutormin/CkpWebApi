using CkpDAL.Entities.Module;
using CkpDAL.Entities.String;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_position_ims")]
    public class PositionIm
    {
		[Key]
		[Required]
		[Column("order_position_id", TypeName = "int")]
		public int OrderPositionId { get; set; }

		[ForeignKey("OrderPositionId")]
		public OrderPosition OrderPosition { get; set; }

		[Key]
		[Required]
		[Column("position_im_type_id", TypeName = "int")]
		public int PositionImTypeId { get; set; }
		
		[ForeignKey("PositionImTypeId")]
		public PositionImType PositionImType { get; set; }

		[Required]
		[Column("order_id", TypeName = "int")]
		public int OrderId { get; set; }

		[Column("parent_position_id", TypeName = "int")]
		public int? ParentPositionId { get; set; }

		[Required]
		[Column("number", TypeName = "int")]
		public int Number { get; set; }

		[Required]
		[Column("maket_status_id", TypeName = "int")]
		public int MaketStatusId { get; set; }

		[Required]
		[Column("maket_category_id", TypeName = "int")]
		public int MaketCategoryId { get; set; }

		[Required]
		[Column("text", TypeName = "text")]
		public string Text { get; set; }

		[Required]
		[Column("comments", TypeName = "varchar(4000)")]
		public string Comments { get; set; }

		[Required]
		[Column("url", TypeName = "varchar(500)")]
		public string Url { get; set; }

		[Required]
		[Column("distribution_url", TypeName = "varchar(50)")]
		public string DistributionUrl { get; set; }

		[Column("legal_person_personal_data_id", TypeName = "int")]
		public int? LegalPersonPersonalDataId { get; set; }

		[Column("xml", TypeName = "text")]
		public string Xml { get; set; }

		[Required]
		[Column("rating", TypeName = "int")]
		public int Rating { get; set; }

		[Required]
		[Column("rating_description", TypeName = "varchar(500)")]
		public string RatingDescription { get; set; }

		[Required]
		[Column("dont_verify", TypeName = "bit")]
		public bool DontVerify { get; set; }

		[Required]
		[Column("rdv_rating", TypeName = "bit")]
		public bool RdvRating { get; set; }

		[Column("task_file_date", TypeName = "datetime")]
		public DateTime? TaskFileDate { get; set; }

		[Column("maket_file_date", TypeName = "datetime")]
		public DateTime? MaketFileDate { get; set; }

		[Required]
		[Column("im_replace_status_id", TypeName = "int")]
		public int ImReplaceStatusId { get; set; }

		[Required]
		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }

		[ForeignKey("OrderPositionId")]
		public StringPosition StringPosition { get; set; }

		[ForeignKey("OrderPositionId")]
		public ModulePosition ModulePosition { get; set; }
	}
}
