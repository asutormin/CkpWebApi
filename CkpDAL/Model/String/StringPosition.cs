using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model.String
{
    [Table("vwa_strings")]
	public class StringPosition
	{
		[Key]
		[Required]
		[Column("string_id", TypeName = "int")]
		public int Id { get; set; }

		[Required]
		[Column("company_id", TypeName = "int")]
		public int CompanyId { get; set; }

		[Column("anonymous_company_name", TypeName = "varchar(100)")]
		public string AnonymousCompanyName { get; set; }

		[Required]
		[Column("business_unit_id", TypeName = "int")]
		public int BusinessUnitId { get; set; }

		[Column("string_date", TypeName = "datetime")]
		public DateTime? Date { get; set; }

		[Required]
		[Column("vacancy_name", TypeName = "varchar(50)")]
		public string VacancyName { get; set; }

		[Column("vacancy_additional", TypeName = "varchar(100)")]
		public string VacancyAdditional { get; set; }

		[Column("vacancy_code", TypeName = "varchar(5)")]
		public string VacancyCode { get; set; }

		[Column("responsibility", TypeName = "varchar(2000)")]
		public string Responsibility { get; set; }

		[Column("requirement", TypeName = "varchar(2000)")]
		public string Requirement { get; set; }

		[Column("condition", TypeName = "varchar(2000)")]
		public string Condition { get; set; }

		[Column("salary_from", TypeName = "money")]
		public float? SalaryFrom { get; set; }

		[Column("salary_to", TypeName = "money")]
		public float? SalaryTo { get; set; }

		[Column("salary_description", TypeName = "varchar(100)")]
		public string SalaryDescription { get; set; }

		[Column("currency_id", TypeName = "int")]
		public int? CurrencyId { get; set; }

		[Column("work_graphic_id", TypeName = "int")]
		public int? WorkGraphicId { get; set; }

		[Required]
		[Column("work_graphic_com", TypeName = "varchar(256)")]
		public string WorkGraphic { get; set; }

		[Column("citizenship_id", TypeName = "int")]
		public int? CitizenshipId { get; set; }

		[Required]
		[Column("job_vacancy_level", TypeName = "int")]
		public int JobVacancyLevel { get; set; }

		[Column("work_pressure_id", TypeName = "int")]
		public int? WorkPressureId { get; set; }

		[Column("job_place_id", TypeName = "int")]
		public int? JobPlaceId { get; set; }

		[Column("age_from", TypeName = "int")]
		public int? AgeFrom { get; set; }

		[Column("age_to", TypeName = "int")]
		public int? AgeTo { get; set; }

		[Column("gender_id", TypeName = "int")]
		public int? GenderId { get; set; }

		[Column("education_id", TypeName = "int")]
		public int? EducationId { get; set; }

		[Column("experience_id", TypeName = "int")]
		public int? ExperienceId { get; set; }

		[Required]
		[Column("experience_value", TypeName = "int")]
		public int ExperienceValue { get; set; }

		[Column("keywords", TypeName = "varchar(300)")]
		public string Keywords { get; set; }

		[Column("logo", TypeName = "image")]
		public byte[] Logo { get; set; }

		[Column("logo_file_name", TypeName = "varchar(100)")]
		public string LogoFileName { get; set; }

		[Column("contact_first_name", TypeName = "varchar(100)")]
		public string ContactFirstName { get; set; }

		[Column("contact_second_name", TypeName = "varchar(100)")]
		public string ContactSecondName { get; set; }

		[Column("contact_last_name", TypeName = "varchar(100)")]
		public string ContactLastName { get; set; }

		[Column("text", TypeName = "text")]
		public string Text { get; set; }

		[Required]
		[Column("order_position_id", TypeName = "int")]
		public int OrderPositionId { get; set; }

		[Required]
		[Column("is_salary_percent", TypeName = "bit")]
		public bool IsSalaryPercent { get; set; }

		[Required]
		[Column("is_housing", TypeName = "bit")]
		public bool IsHousing { get; set; }

		[Required]
		[Column("is_food", TypeName = "bit")]
		public bool IsFood { get; set; }

		[ForeignKey("OrderPositionId")]
		public PositionIm PositionIm { get; set; }

		[ForeignKey("OrderPositionId")]
		public OrderPosition OrderPosition { get; set; }

		[Required]
		[Column("begin_date", TypeName = "datetime")]
		public DateTime BeginDate { get; set; }

		[Required]
		[Column("edit_user_id", TypeName = "int")]
		public int EditUserId { get; set; }

		public virtual ICollection<StringAddress> Addresses { get; set; }
		public virtual ICollection<StringOccurrence> Occurrences { get; set; }
		public virtual ICollection<StringPhone> Phones { get; set; }
		public virtual ICollection<StringWeb> Webs { get; set; }
	}
}
