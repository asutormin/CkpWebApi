using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_account_orders")]
    public class AccountOrder
    {
        [Key]
        [Column("account_order_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("account_id", TypeName = "int")]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        [Column("order_id", TypeName = "int")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
