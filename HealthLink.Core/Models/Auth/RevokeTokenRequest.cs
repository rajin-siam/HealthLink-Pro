using System.ComponentModel.DataAnnotations;

namespace HealthLink.Core.Models.Auth
{
    /// <summary>
    /// Request model for revoking a refresh token (logout).
    /// </summary>
    public class RevokeTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}