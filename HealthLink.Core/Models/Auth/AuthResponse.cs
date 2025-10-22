namespace HealthLink.Core.Models.Auth
{
    /// <summary>
    /// Response model for successful authentication.
    /// </summary>
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserInfo User { get; set; }
    }
}
