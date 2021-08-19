using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_rubrics")]
    public class RubricVersionable : RubricBase
    {
        [Key]
        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Column("end_date", TypeName = "datetime")]
        public DateTime EndDate { get; set; }
    }
}
