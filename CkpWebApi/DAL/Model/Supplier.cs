using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_suppliers")]
    public class Supplier
    {
        [Key]
        [Column("company_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("parent_company_id", TypeName = "int")]
        public int ParentId { get; set; }

        [ForeignKey("Id")]
        public Company Company { get; set; }

        [Column("supplier_full_name", TypeName = "varchar(300)")]
        public string FullName { get; set; }

        [Column("is_need_nds", TypeName = "int")]
        public int IsNeedNds { get; set; }
        
        [Column("nds", TypeName = "varchar(150)")]
        public string Nds { get; set; }

        [Column("city_id", TypeName = "int")]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        public virtual ICollection<PricePositionActual> PricePositions { get; set; }

        public virtual ICollection<Graphic> Graphics { get; set; }
    }
}
