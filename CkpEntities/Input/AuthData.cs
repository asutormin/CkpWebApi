using System.ComponentModel.DataAnnotations;

namespace CkpModel.Input
{
    public class AuthData
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
