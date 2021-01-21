using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model.String
{
    [Table("t_s_string_phones")]
    public class StringPhone
    {
        [Key]
        [Required]
        [Column("string_id", TypeName = "int")]
        public int StringId { get; set; }

        [ForeignKey("StringId")]
        public StringPosition StringPosition { get; set; }

        [Key]
        [Required]
        [Column("phone_id", TypeName = "int")]
        public int PhoneId { get; set; }

        [Column("phone_type_id", TypeName = "int")]
        public int? PhoneTypeId { get; set; }

        [Required]
        [Column("country_code", TypeName = "varchar(10)")]
        public string CountryCode { get; set; }

        [Column("code", TypeName = "varchar(10)")]
        public string Code { get; set; }

        [Column("number", TypeName = "varchar(10)")]
        public string Number { get; set; }

        [Column("additional_number", TypeName = "varchar(10)")]
        public string AdditionalNumber { get; set; }

        [Column("description", TypeName = "varchar(300)")]
        public string Description { get; set; }

        [Required]
        [Column("order_by", TypeName = "int")]
        public int OrderBy { get; set; }

        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
