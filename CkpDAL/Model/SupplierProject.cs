using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_supplier_projects")]
    public class SupplierProject
    {
		[Required]
		[Key]
		[Column("supplier_project_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("company_id", TypeName = "int")]
		public int SupplierId { get; set; }

		[Required]
		[Column("supplier_project_name", TypeName = "varchar(50)")]
		public string Name { get; set; }

		[Required]
		[Column("supplier_project_type_id", TypeName = "int")]
		public int TypeId { get; set; }

		[Required]
		[Column("creation_date", TypeName = "datetime")]
		public DateTime CreationDate { get; set; }

		[Required]
		[Column("is_active", TypeName = "bit")]
		public bool IsActive { get; set; }

		[Required]
		[Column("actual_begin", TypeName = "datetime")]
		public DateTime ActualBegin { get; set; }

		[Required]
		[Column("actual_end", TypeName = "datetime")]
		public DateTime ActualEnd { get; set; }

		[Required]
		[Column("use_main_graphics", TypeName = "bit")]
		public bool UseMainGraphics { get; set; }

		[Required]
		[Column("use_main_rubrics", TypeName = "bit")]
		public bool UseMainRubrics { get; set; }

		[Required]
		[Column("exclusive_rubrics", TypeName = "bit")]
		public bool ExclusiveRubrics { get; set; }

		[Required]
		[Column("enable_reservations", TypeName = "bit")]
		public bool EnableReservations { get; set; }

		[Required]
		[Column("only_reservation", TypeName = "bit")]
		public bool OnlyReservation { get; set; }

		[Required]
		[Column("order_by", TypeName = "int")]
		public int OrderBy { get; set; }

		[Required]
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
