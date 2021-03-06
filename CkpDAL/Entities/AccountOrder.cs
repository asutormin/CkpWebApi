using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
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

        [Column("account_order_begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }
    }
}
