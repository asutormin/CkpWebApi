using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("t_units")]
    public class Unit
    {
        [Key]
        [Column("unit_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("unit_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Column("is_actual", TypeName = "bit")]
        public bool IsActual { get; set; }
    }
}
