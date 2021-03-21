using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("vwa_supplier_project_rubrics")]
    public class SupplierProjectRubric
    {
        [Required]
        [Key]
        [Column("supplier_project_rubric_id", TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column("supplier_project_id", TypeName = "int")]
        public int SupplierProjectId { get; set; }

        [Required]
        [Column("rubric_id", TypeName = "int")]
        public int RubricId { get; set; }

        [Required]
        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Required]
        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }

        [ForeignKey("SupplierProjectId")]
        public SupplierProject SupplierProject { get; set; }

        [ForeignKey("RubricId")]
        public RubricActual Rubric { get; set; }
    }
}
