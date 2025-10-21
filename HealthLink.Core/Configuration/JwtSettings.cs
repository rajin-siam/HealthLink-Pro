using System.Text;

namespace HealthLink.Core.Configuration
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 60;
        public double RefreshTokenExpirationDays { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
                throw new ArgumentException("JWT SecretKey is required");

            if (string.IsNullOrWhiteSpace(Issuer))
                throw new ArgumentException("JWT Issuer is required");

            if (string.IsNullOrWhiteSpace(Audience))
                throw new ArgumentException("JWT Audience is required");

            if (AccessTokenExpirationMinutes <= 0)
                throw new ArgumentException("AccessTokenExpirationMinutes must be greater than 0");

            // Validate secret key length for HS256 (should be at least 16 bytes for security)
            if (Encoding.UTF8.GetBytes(SecretKey).Length < 16)
                throw new ArgumentException("JWT SecretKey must be at least 16 characters long for security");
        }
    }
}