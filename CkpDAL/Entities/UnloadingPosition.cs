using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_unloading_positions")]
    public class UnloadingPosition
    {
        [Key]
        [Column("unloading_position_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("graphic_position_id", TypeName = "int")]
        public int GraphicPositionId { get; set; }

        [ForeignKey("GraphicPositionId")]
        public GraphicPosition GraphicPosition { get; set; }
    }
}
