using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CkpDAL.Model
{
    [Table("t_legal_person_signs")]
    public class LegalPersonSign
    {
        [Key]
        [Column("legal_person_sign_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("legal_person_id", TypeName = "int")]
        public int LegalPersonId { get; set; }

        [ForeignKey("LegalPersonId")]
        public LegalPerson LegalPerson { get; set; }

        [Column("director_sign_text", TypeName = "varchar(200)")]
        public string DirectorSignText { get; set; }

        [Column("accountant_sign_text", TypeName = "varchar(200)")]
        public string AccountantSignText { get; set; }

        [Column("actual_begin", TypeName = "datetime")]
        public DateTime ActualBegin { get; set; }

        [Column("actual_end", TypeName = "datetime")]
        public DateTime ActualEnd { get; set; }

        public virtual ICollection<LegalPersonSignImage> SignImages { get; set; }

        public LegalPersonSignImage GetImageByMask(int mask)
        {
            return SignImages.FirstOrDefault(signImage => (signImage.SignImageTypeMask & mask) == mask);
        }
    }
}
