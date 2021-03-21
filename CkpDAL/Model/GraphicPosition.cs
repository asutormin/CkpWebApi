using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_graphic_positions")]
    public class GraphicPosition
    {
        [Key]
        [Column("graphic_position_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("parent_graphic_position_id", TypeName = "int")]
        public int ParenGraphicPositiontId { get; set; }

        [ForeignKey("ParenGraphicPositiontId")]
        public virtual GraphicPosition ParentGraphicPosition { get; set; }

        [Column("order_position_id", TypeName = "int")]
        public int OrderPositionId { get; set; }

        [ForeignKey("OrderPositionId")]
        public OrderPosition OrderPosition { get; set; }

        [Column("count_position", TypeName = "int")]
        public int Count { get; set; }

        [Column("graphic_id", TypeName = "int")]
        public int GraphicId { get; set; }

        [ForeignKey("GraphicId")]
        public Graphic Graphic { get; set; }

        public UnloadingPosition UnloadingPosition { get; set; }

        public virtual ICollection<GraphicPosition> ChildGraphicPositions { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
    }
}
