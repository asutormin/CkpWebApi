using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpWebApi.DAL.Model
{
    [Table("vwa_price_positions")]
    public class PricePositionActual : PricePositionBase
    {
        [Column("price_position_description", TypeName = "varchar(150)")]
        public string Description { get; set; }

        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
        public virtual ICollection<Price> Prices { get; set; }
        public virtual ICollection<PackagePosition> PackagePositions { get; set; }
    }
}
