using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_payment_types")]
    public class PaymentType
    {
        [Key]
        [Column("payment_type_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("parent_payment_type_id", TypeName = "int")]
        public int? ParentPaymentTypeId { get; set; }

        [Column("kind_payment_type_id", TypeName = "int")]
        public int? KindPaymentTypeId { get; set; }

        [Required]
        [Column("payment_name", TypeName = "varchar(50)")]
        public string PaymentName { get; set; }

        [Required]
        [Column("payment_factor", TypeName = "int")]
        public int PaymentFactor { get; set; }

        [Column("description", TypeName = "varchar(300)")]
        public string Description { get; set; }
    }
}
