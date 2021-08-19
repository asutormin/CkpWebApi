using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("vwa_companies")]
    public class Company
    {
        [Key]
        [Column("company_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("company_name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        public virtual ICollection<BusinessUnitCompanyManager> BusinessUnitManagers { get; set; }
    }
}
