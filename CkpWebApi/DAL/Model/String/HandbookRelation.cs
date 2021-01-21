using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model.String
{
    [Table("t_s_handbook_relations")]
    public class HandbookRelation
    {
        [Key]
        [Required]
        [Column("company_id", TypeName = "int")]
        public int CompanyId { get; set; }

        [Key]
        [Required]
        [Column("handbook_id", TypeName = "int")]
        public int HandbookId { get; set; }

        [Key]
        [Required]
        [Column("handbook_type_id", TypeName = "int")]
        public int HandbookTypeId { get; set; }

        [Required]
        [Column("supplier_value", TypeName = "varchar(500)")]
        public string SupplierValue { get; set; }

        [Column("supplier_value_id", TypeName = "int")]
        public int? SupplierValueId { get; set; }

        [Column("description", TypeName = "varchar(100)")]
        public string Description { get; set; }
    }
}
