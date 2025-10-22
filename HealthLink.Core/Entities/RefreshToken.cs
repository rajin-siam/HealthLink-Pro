using HealthLink.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string RefreshTokenValue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string IpAddress { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public RefreshToken()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public RefreshToken(Guid userId, string refreshToken, DateTime expiryDate, string ipAddress)
        : this()
    {
        UserId = userId;
        RefreshTokenValue = refreshToken;
        ExpiryDate = expiryDate;
        IpAddress = ipAddress;
    }
}