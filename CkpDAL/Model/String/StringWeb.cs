using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model.String
{
    [Table("t_s_string_webs")]
    public class StringWeb
    {
        [Key]
        [Required]
        [Column("web_id", TypeName = "int")]
        public int WebId { get; set; }

        [Key]
        [Required]
        [Column("string_id", TypeName = "int")]
        public int StringId { get; set; }

        [ForeignKey("StringId")]
        public StringPosition StringPosition { get; set; }

        [Required]
        [Column("web_type_id", TypeName = "int")]
        public int WebTypeId { get; set; }

        [Required]
        [Column("web_response", TypeName = "int")]
        public int WebResponse { get; set; }

        [Required]
        [Column("web_value", TypeName = "varchar(50)")]
        public string WebValue { get; set; }

        [Column("description", TypeName = "varchar(300)")]
        public string Description { get; set; }

        [Required]
        [Column("order_by", TypeName = "int")]
        public int OrderBy { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
