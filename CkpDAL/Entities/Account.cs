using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_accounts")]
    public class Account
    {
        [Key]
        [Column("account_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("number", TypeName = "varchar(10)")]
        public string Number { get; set; }

        [Column("account_date", TypeName = "datetime")]
        public DateTime Date { get; set; }

        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [Column("legal_person_id", TypeName = "int")]
        public int LegalPersonId { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

        [Column("cash_id", TypeName = "int")]
        public int CashId { get; set; }

        [ForeignKey("CashId")]
        public Cash Cash { get; set; }

        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public BusinessUnit BusinessUnit { get; set; }

        [Column("account_status_id", TypeName = "int")]
        public int StatusId { get; set; }

        [Column("account_type_id", TypeName = "int")]
        public int TypeId { get; set; }

        [Column("nds", TypeName = "float")]
        public float Nds { get; set; }

        [Column("description", TypeName = "varchar(max)")]
        public string Description { get; set; }

        [Column("additional_description", TypeName = "varchar(300)")]
        public string AdditionalDescription { get; set; }

        [Column("account_sum", TypeName = "money")]
        public float Sum { get; set; }

        [Column("prepaid_sum", TypeName = "money")]
        public float Prepaid { get; set; }

        [Column("debt_sum", TypeName = "money")]
        public float Debt { get; set; }

        [Column("payment_agent_id", TypeName = "int")]
        public int PaymentAgentId { get; set; }

        [Column("payment_agent_commission_sum", TypeName = "money")]
        public float PaymentAgentCommissionSum { get; set; }

        [Column("request", TypeName = "varchar(max)")]
        public string Request { get; set; }

        [Column("printed", TypeName = "bit")]
        public bool Printed { get; set; }

        [Column("unloaded_to_1c", TypeName = "bit")]
        public bool UnloadedTo1C { get; set; }

        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        public ICollection<AccountPosition> AccountPositions { get; set; }

        public AccountSettings AccountSettings { get; set; }

        public virtual ICollection<AccountOrder> AccountOrders { get; set; }
    }
}
