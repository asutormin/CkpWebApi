using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("t_metros")]
    public class Metro
    {
        [Key]
        [Required]
        [Column("metro_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("metro_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("metro_line_id", TypeName = "int")]
        public int MetroLineId { get; set; }

        [Required]
        [Column("region_id", TypeName = "int")]
        public int RegionId { get; set; }

        [Required]
        [Column("city_id", TypeName = "int")]
        public int CityId { get; set; }

        [Required]
        [Column("type_id", TypeName = "int")]
        public int TypeId { get; set; }

        [Required]
        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }

    }
}
