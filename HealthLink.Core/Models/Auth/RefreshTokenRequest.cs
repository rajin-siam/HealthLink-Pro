using System.ComponentModel.DataAnnotations;


namespace HealthLink.Core.Models.Auth
{
    /// <summary>
    /// Request model for token refresh.
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }

}
