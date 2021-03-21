using System.ComponentModel.DataAnnotations;

namespace CkpEntities.Input
{
    public class LoginInfo
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
