using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Entities.String
{
    [Table("vwa_string_occurrences")]
    public class StringOccurrence
    {
        [Key]
        [Required]
        [Column("string_id", TypeName = "int")]
        public int StringId { get; set; }

        [ForeignKey("StringId")]
        public StringPosition StringPosition { get; set; }

        [Key]
        [Required]
        [Column("occurrence_id", TypeName = "int")]
        public int OccurrenceId { get; set; }

        [Key]
        [Required]
        [Column("type_id", TypeName = "int")]
        public int TypeId { get; set; }

        [Key]
        [Required]
        [Column("begin_date", TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Required]
        [Column("order_by", TypeName = "int")]
        public int OrderBy { get; set; }

        [Required]
        [Column("edit_user_id", TypeName = "int")]
        public int EditUserId { get; set; }
    }
}
