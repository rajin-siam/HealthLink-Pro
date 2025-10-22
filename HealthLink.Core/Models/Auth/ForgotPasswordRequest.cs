using System.ComponentModel.DataAnnotations;


namespace HealthLink.Core.Models.Auth
{
    /// <summary>
    /// Request model for password reset.
    /// </summary>
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
