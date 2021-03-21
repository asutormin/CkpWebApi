using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_order_positions_rubrics")]
    public class RubricPosition
    {
        [Key]
        [Column("order_positions_rubrics_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("order_position_id", TypeName = "int")]
        public int OrderPositionId { get; set; }

        [ForeignKey("OrderPositionId")]
        public OrderPosition OrderPosition { get; set; }

        [Column("rubric_id", TypeName = "int")]
        public int RubricId { get; set; }

        [Column("rubric_version_date", TypeName = "datetime")]
        public DateTime RubricVersion { get; set; }

        [ForeignKey("RubricId, RubricVersion")]
        public RubricVersionable Rubric { get; set; }
    }
}
