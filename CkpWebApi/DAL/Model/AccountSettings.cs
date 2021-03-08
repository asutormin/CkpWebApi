using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_account_settings")]
    public class AccountSettings
    {
        [Key]
        [Column("account_setting_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("account_id", TypeName = "int")]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

		[Column("legal_person_id", TypeName = "int")]
		public int LegalPersonId { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

		[Column("bank_id", TypeName = "int")]
		public int LegalPersonBankId { get; set; }

        [ForeignKey("LegalPersonBankId")]
        public LegalPersonBank LegalPersonBank { get; set; }

		[Column("unloading_date_method", TypeName = "int")]
		public int UnloadingDateMethod { get; set; }

		[Column("additional_description", TypeName = "varchar(500)")]
		public string AdditionalDescription { get; set; }

		[Column("account_description", TypeName = "varchar(1000)")]
		public string AccountDescription { get; set; }

		[Column("act_description", TypeName = "varchar(1000)")]
		public string ActDescription { get; set; }

		[Column("short_account_position_formulation", TypeName = "varchar(150)")]
		public string ShortAccountPositionFormulation { get; set; }

		[Column("advance_account_position_formulation", TypeName = "varchar(150)")]
		public string AdvanceAccountPositionFormulation { get; set; }

		[Column("contract_number", TypeName = "varchar(20)")]
		public string ContractNumber { get; set; }

		[Column("contract_date", TypeName = "varchar(10)")]
		public string ContractDate { get; set; }

		[Column("show_additional_description", TypeName = "bit")]
		public bool ShowAdditionalDescription { get; set; }

		[Column("show_detailed", TypeName = "bit")]
		public bool ShowDetailed { get; set; }

		[Column("show_contract_in_caption", TypeName = "bit")]
		public bool ShowContractInCaption { get; set; }

		[Column("show_okpo", TypeName = "bit")]
		public bool ShowOkpo { get; set; }

		[Column("show_supplier", TypeName = "bit")]
		public bool ShowSupplier { get; set; }

		[Column("show_position_name", TypeName = "bit")]
		public bool ShowPositionName { get; set; }

		[Column("show_exit_date", TypeName = "bit")]
		public bool ShowExitDate{ get; set; }

		[Column("show_exit_number", TypeName = "bit")]
		public bool ShowExitNumber{ get; set; }

		[Column("show_contract", TypeName = "bit")]
		public bool ShowContract { get; set; }

		[Column("show_discount", TypeName = "bit")]
		public bool ShowDiscount { get; set; }

		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }
	}
}
