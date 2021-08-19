using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.String
{
    [Table("t_s_string_addresses")]
    public class StringAddress
    {
        [Key]
        [Required]
        [Column("string_id", TypeName = "int")]
        public int StringId { get; set; }

        [ForeignKey("StringId")]
        public StringPosition StringPosition { get; set; }

        [Key]
        [Required]
        [Column("address_id", TypeName = "int")]
        public int AddressId { get; set; }

        [Column("city_id", TypeName = "int")]
        public int? CityId { get; set; }

        [Column("metro_id", TypeName = "int")]
        public int? MetroId { get; set; }

        [Column("street", TypeName = "varchar(150)")]
        public string Street { get; set; }

        [Column("house", TypeName = "varchar(10)")]
        public string House { get; set; }

        [Column("corps", TypeName = "varchar(10)")]
        public string Corps { get; set; }

        [Column("building", TypeName = "varchar(10)")]
        public string Building { get; set; }

        [Column("description", TypeName = "varchar(500)")]
        public string Description { get; set; }

        [Required]
        [Column("order_by", TypeName = "int")]
        public int OrderBy { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
