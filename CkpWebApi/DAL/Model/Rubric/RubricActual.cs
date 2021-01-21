using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_rubrics")]
    public class RubricActual : RubricBase
    {
        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
    }
}
