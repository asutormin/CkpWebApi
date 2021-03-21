using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
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

        [Column("legal_person_id", TypeName = "int")]
        public int ClientLegalPersonId { get; set; }

        [ForeignKey("ClientLegalPersonId")]
        public LegalPerson ClientLegalPerson { get; set; }

        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public BusinessUnit BusinessUnit { get; set; }

        [Column("nds", TypeName = "float")]
        public float Nds { get; set; }

        [Column("account_sum", TypeName = "money")]
        public float Sum { get; set; }

        [Column("prepaid_sum", TypeName = "money")]
        public float Prepaid { get; set; }

        [Column("debt_sum", TypeName = "money")]
        public float Debt { get; set; }

        [Column("request", TypeName = "varchar(max)")]
        public string Request { get; set; }

        public ICollection<AccountPosition> AccountPositions { get; set; }

        public AccountSettings AccountSettings { get; set; }

        public virtual ICollection<AccountOrder> AccountOrders { get; set; }
    }
}
