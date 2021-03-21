using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CkpDAL.Model
{
    [Table("t_legal_person_sign_images")]
    public class LegalPersonSignImage
    {
        [Key]
        [Column("sign_image_id", TypeName = "int")]
        public int Id { get; set; }

        [Column("legal_person_sign_id", TypeName = "int")]
        public int LegalPersonSignId { get; set; }

        [ForeignKey("LegalPersonSignId")]
        public LegalPersonSign LegalPersonSign { get; set; }

        [Column("sign_image_type_mask", TypeName = "int")]
        public int SignImageTypeMask { get; set; }

        [Column("sign_image", TypeName = "image")]
        public byte[] SignImage { get; set; }
    }
}
