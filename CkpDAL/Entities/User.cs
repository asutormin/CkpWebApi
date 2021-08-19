using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_users")]
    public class User
    {
		[Key]
		[Column("user_id", TypeName = "int")]
		public int Id { get; set; }

		[Column("user_name", TypeName = "varchar(50)")]
		public string Name { get; set; }
    }
}
