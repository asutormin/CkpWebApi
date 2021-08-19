using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_business_unit_company_managers")]
    public class BusinessUnitCompanyManager
    {
        [Key]
        [Column("business_unit_company_manager_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("business_unit_id", TypeName = "int")]
        public int BusinessUnitId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public BusinessUnit BusinessUnit { get; set; }

        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [Column("manager_id", TypeName = "int")]
        public int ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public User Manager { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
    }
}
