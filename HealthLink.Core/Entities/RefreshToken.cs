using System;
using HealthLink.Core.Entities;

namespace HealthLink.Core.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string? RefreshTokenValue { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? IpAddress { get; set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Guid UserId { get; set; }
        public bool IsUsed { get; private set; } = false;
        public DateTime? UsedAt { get; private set; }

        public bool IsRevoked { get; private set; } = false;
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }

        // Navigation property
        public virtual User? User { get; set; }

        // Default constructor - sets Id and timestamps
        public RefreshToken()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        // Overloaded constructor - for creating new tokens
        public RefreshToken(Guid userId, string refreshToken, DateTime expiryDate, string? ipAddress)
            : this()
        {
            UserId = userId;
            RefreshTokenValue = refreshToken;
            ExpiryDate = expiryDate;
            IpAddress = ipAddress;
        }

        // Marks the token as used (e.g., when a new access token is generated)
        public void MarkAsUsed()
        {
            if (IsUsed) throw new InvalidOperationException("Token has already been used.");
            if (IsRevoked) throw new InvalidOperationException("Token has been revoked and cannot be used.");

            IsUsed = true;
            UsedAt = DateTime.UtcNow;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Revokes the token manually (e.g., on logout)
        public void Revoke(string ipAddress)
        {
            if (IsRevoked) throw new InvalidOperationException("Token is already revoked.");

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = ipAddress;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Convenience property - helps in checking if token is still valid
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    }
}
