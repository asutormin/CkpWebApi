using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.Module
{
    [Table("vwa_modules")]
    public class ModulePosition
    {
        [Key]
        [Column("module_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

        [Required]
        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }

        [Required]
        [Column("order_position_id", TypeName = "int")]
        public int OrderPositionId { get; set; }

        [Column("vacancy_name", TypeName = "varchar(500)")]
        public string VacancyName { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }
    }
}
