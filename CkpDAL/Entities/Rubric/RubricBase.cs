using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_rubrics")]
    public abstract class RubricBase
    {
        [Key]
        [Column("rubric_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("company_id", TypeName = "int")]
        public int SupplierId { get; set; }
        
        [Column("parent_rubric_id", TypeName = "int")]
        public int? ParentRubricId { get; set; }
        
        [Column("rubric_number", TypeName = "varchar(20)")]
        public string Number { get; set; }

        [Column("rubric_name", TypeName = "varchar(500)")]
        public string Name { get; set; }

        [Column("order_by", TypeName = "int")]
        public int OrderBy { get; set; }
    }
}
