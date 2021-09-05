using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.String
{
    [Table("t_s_addresses")]
    public class Address
    {
        [Key]
        [Required]
        [Column("address_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

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
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }

        [Required]
        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }
    }
}
