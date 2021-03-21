using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model.String
{
	[Table("t_s_phones")]
	public class Phone
	{
		[Key]
		[Required]
		[Column("phone_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("company_id", TypeName = "int")]
		public int CompanyId { get; set; }

		[Required]
		[Column("phone_type_id", TypeName = "int")]
		public int PhoneTypeId { get; set; }

		[ForeignKey("PhoneTypeId")]
		public virtual PhoneType PhoneType {get; set;}

		[Required]
		[Column("country_code", TypeName = "varchar(10)")]
		public string CountryCode { get; set; }

		[Required]
		[Column("code", TypeName = "varchar(10)")]
		public string Code { get; set; }

		[Required]
		[Column("number", TypeName = "varchar(10)")]
		public string Number { get; set; }

		[Column("additional_number", TypeName = "varchar(10)")]
		public string AdditionalNumber { get; set; }

		[Column("description", TypeName = "varchar(300)")]
		public string Description { get; set; }

		[Required]
		[Column("is_actual", TypeName = "bit")]
		public bool IsActual { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }
	}
}
