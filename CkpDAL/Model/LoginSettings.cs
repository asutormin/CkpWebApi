using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("t_legal_person_login_settings")]
    public class LoginSettings
    {
        [Key]
        [Column("legal_person_login_settings_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("legal_person_id", TypeName = "int")]
        public int LegalPersonId { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

        [Column("login", TypeName = "varchar(50)")]
        public string Login { get; set; }

        [Column("password_md5", TypeName = "varchar(50)")]
        public string PasswordMd5 { get; set; }

        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
