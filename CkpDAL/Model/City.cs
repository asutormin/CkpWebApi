using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("t_cities")]
    public class City
    {
        [Key]
        [Required]
        [Column("city_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("city_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("type_id", TypeName = "int")]
        public int TypeId { get; set; }

        [Required]
        [Column("is_show_for_distribution", TypeName = "bit")]
        public bool IsShowForDistribution { get; set; }
    }
}
