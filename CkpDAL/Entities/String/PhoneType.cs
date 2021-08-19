using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.String
{
    [Table("t_phone_types")]
    public class PhoneType
    {
        [Key]
        [Required]
        [Column("phone_type_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("phone_type_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
