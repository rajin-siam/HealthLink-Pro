using System.ComponentModel.DataAnnotations;

namespace HealthLink.Core.Models.Auth
{
    /// <summary>
    /// Request model for user login.
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username or email is required")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
