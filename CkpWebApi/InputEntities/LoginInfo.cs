using System.ComponentModel.DataAnnotations;

namespace CkpWebApi.InputEntities
{
    public class LoginInfo
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
