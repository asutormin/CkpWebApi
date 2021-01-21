using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_supplier_project_relations")]
    public class SupplierProjectRelation
    {
		[Required]
		[Key]
		[Column("supplier_project_relation_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("supplier_project_id", TypeName = "int")]
		public int SupplierProjectId { get; set; }

		[Column("price_position_id", TypeName = "int")]
		public int PricePositionId { get; set; }

		[Column("price_id", TypeName = "int")]
		public int PriceId { get; set; }

		[Required]
		[Column("reservation_count", TypeName = "int")]
		public int ReservationCount { get; set; }

		[Required]
		[Column("reservation_count_current", TypeName = "int")]
		public int ReservationCountCurrent { get; set; }

		[Required]
		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }

		[ForeignKey("SupplierProjectId")]
		public SupplierProject SupplierProject { get; set; }

		[ForeignKey("PriceId")]
		public Price Price { get; set; }

		[ForeignKey("PricePositionId")]
		public PricePositionActual PricePosition { get; set; }
	}
}
