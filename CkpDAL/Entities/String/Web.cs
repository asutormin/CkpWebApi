using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.String
{
    [Table("t_s_webs")]
    public class Web
    {
        [Key]
        [Column("web_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

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
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }

        [Required]
        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }
    }
}
