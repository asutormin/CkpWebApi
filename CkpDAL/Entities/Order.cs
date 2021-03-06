using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_orders")]
    public class Order
    {
        [Key]
        [Column("order_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("parent_order_id", TypeName = "int")]
        public int? ParentOrderId { get; set; }

        [Required]
        [Column("activity_type_id", TypeName = "int")]
        public int ActivityTypeId { get; set; }

        [Required]
        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public BusinessUnit BusinessUnit { get; set; }

        [Column("status_id", TypeName = "int")]
        public int? StatusId { get; set; }

        [Column("visa", TypeName = "bit")]
        public bool? IsNeedVisa { get; set; }

        [Column("is_need_account", TypeName = "bit")]
        public bool? IsNeedAccount { get; set; }

        [Column("account_description", TypeName = "varchar(max)")]
        public string AccountDescription { get; set; }

        [Required]
        [Column("order_number", TypeName = "varchar(50)")]
        public string OrderNumber { get; set; }

        [Required]
        [Column("order_date", TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        [Column("max_exit_date", TypeName = "datetime")]
        public DateTime? MaxExitDate { get; set; }

        [Required]
        [Column("company_id", TypeName = "int")]
        public int ClientCompanyId { get; set; }

        [ForeignKey("ClientCompanyId")]
        public Company ClientCompany { get; set; }

        [Required]
        [Column("client_legal_person_id", TypeName = "int")]
        public int ClientLegalPersonId { get; set; }

        [ForeignKey("ClientLegalPersonId")]
        public LegalPerson ClientLegalPerson { get; set; }

        [Required]
        [Column("supplier_legal_person_id", TypeName = "int")]
        public int SupplierLegalPersonId { get; set; }

        [ForeignKey("SupplierLegalPersonId")]
        public LegalPerson SupplierLegalPerson { get; set; }

        [Required]
        [Column("order_sum", TypeName = "money")]
        public float Sum { get; set; }

        [Required]
        [Column("order_paid", TypeName = "money")]
        public float Paid { get; set; }

        [Required]
        [Column("is_cashless", TypeName = "bit")]
        public bool IsCashless { get; set; }

        [Required]
        [Column("is_advance", TypeName = "bit")]
        public bool IsAdvance { get; set; }

        [Column("description", TypeName = "varchar(max)")]
        public string Description { get; set; }

        [Column("request", TypeName = "varchar(max)")]
        public string Request { get; set; }

        [Required]
        [Column("manager_id", TypeName = "int")]
        public int ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public User Manager { get; set; }

        [Required]
        [Column("is_payment_with_agent", TypeName = "bit")]
        public bool IsPaymentWithAgent { get; set; }

        [Required]
        [Column("is_factoring", TypeName = "bit")]
        public bool IsFactoring { get; set; }

        [Required]
        [Column("created_payment_prognosis_type_id", TypeName = "int")]
        public int CreatedPaymentPrognosisTypeId { get; set; }

        [Required]
        [Column("current_payment_prognosis_type_id", TypeName = "int")]
        public int CurrentPaymentPrognosisTypeId { get; set; }

        [Column("payment_arbitary_prognosis_date", TypeName = "datetime")]
        public DateTime? PaymentArbitaryPrognosisDate { get; set; }

        [Required]
        [Column("order_begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Required]
        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }

        public AccountOrder AccountOrder { get; set; }

        public virtual ICollection<OrderIm> OrderIms { get; set; }

        public virtual ICollection<OrderPosition> OrderPositions { get; set; }

        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
