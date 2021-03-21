using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model.String
{
    [Table("t_s_handbooks")]
    public class Handbook
    {
        [Key]
        [Required]
        [Column("handbook_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("handbook_value", TypeName = "varchar(100)")]
        public string HandbookValue { get; set; }

        [Required]
        [Column("handbook_type_id", TypeName = "int")]
        public int HandbookTypeId { get; set; }
    }
}
