using Domain.Misc;
using System.ComponentModel.DataAnnotations;

namespace Chandler.Controllers.Accounts.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is Required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public ServerConfig ServerConfig { get; set; }
    }
}
