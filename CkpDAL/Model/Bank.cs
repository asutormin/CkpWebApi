using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_banks")]
    public class Bank
    {
        [Key]
        [Column("bank_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("bank_name", TypeName = "varchar(200)")]
        public string Name { get; set; }

        [Column("bik", TypeName = "varchar(9)")]
        public string Bik { get; set; }

        [Column("correspondent_account", TypeName = "varchar(20)")]
        public string CorrespondentAccount { get; set; }
    }
}
