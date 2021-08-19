using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities
{
    [Table("t_price_positions")]
    public class PricePositionVersionable : PricePositionBase
    {
        [Column("description", TypeName = "varchar(150)")]
        public string Description { get; set; }

        [Key]
        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Column("end_date", TypeName = "datetime")]
        public DateTime EndDate { get; set; }
    }
}
