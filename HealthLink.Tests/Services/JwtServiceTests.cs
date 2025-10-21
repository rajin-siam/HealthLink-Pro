using FluentAssertions;
using HealthLink.Business.Services;
using HealthLink.Core.Configuration;
using HealthLink.Core.Constants;
using HealthLink.Core.Entities;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;
using Xunit;

namespace HealthLink.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsAVerySecureSecretKeyForJwtTokenGeneration12345",
                Issuer = "HealthLinkAPI",
                Audience = "HealthLinkClient",
                AccessTokenExpirationMinutes = 60,
                RefreshTokenExpirationDays = 7,
                
            };

            var options = Options.Create(_jwtSettings);
            _jwtService = new JwtService(options);
        }

        [Fact]
        public void GenerateAccessToken_WithValidUser_ShouldReturnToken()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var roles = new List<string> { Roles.Patient };

            // Act
            var token = _jwtService.GenerateAccessToken(user, roles);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
            token.Split('.').Should().HaveCount(3); // JWT has 3 parts
        }

        [Fact]
        public void GenerateAccessToken_TokenShouldContainUserClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("johndoe", "john@example.com", "John Doe");
            var roles = new List<string> { Roles.Patient };

            // Act
            var token = _jwtService.GenerateAccessToken(user, roles);
            var principal = _jwtService.ValidateToken(token);

            // Assert
            principal.Should().NotBeNull();
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(user.Id.ToString());
            principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(user.Email);
            principal.FindFirst(CustomClaims.FullName)?.Value.Should().Be(user.FullName);
            principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be(Roles.Patient);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnUniqueToken()
        {
            // Act
            var token1 = _jwtService.GenerateRefreshToken();
            var token2 = _jwtService.GenerateRefreshToken();

            // Assert
            token1.Should().NotBeNullOrWhiteSpace();
            token2.Should().NotBeNullOrWhiteSpace();
            token1.Should().NotBe(token2);
        }

        [Fact]
        public void ValidateToken_WithValidToken_ShouldReturnPrincipal()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var roles = new List<string> { Roles.Patient };
            var token = _jwtService.GenerateAccessToken(user, roles);

            // Act
            var principal = _jwtService.ValidateToken(token);

            // Assert
            principal.Should().NotBeNull();
            principal.Identity.Should().NotBeNull();
            principal.Identity.IsAuthenticated.Should().BeTrue();
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var principal = _jwtService.ValidateToken(invalidToken);

            // Assert
            principal.Should().BeNull();
        }

        [Fact]
        public void GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var roles = new List<string> { Roles.Patient };
            var token = _jwtService.GenerateAccessToken(user, roles);

            // Act
            var userId = _jwtService.GetUserIdFromToken(token);

            // Assert
            userId.Should().NotBeNull();
            userId.Should().Be(user.Id);
        }

        [Fact]
        public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var userId = _jwtService.GetUserIdFromToken(invalidToken);

            // Assert
            userId.Should().BeNull();
        }

        [Fact]
        public void GetTokenExpirationDate_WithValidToken_ShouldReturnExpirationDate()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var roles = new List<string> { Roles.Patient };
            var token = _jwtService.GenerateAccessToken(user, roles);

            // Act
            var expirationDate = _jwtService.GetTokenExpirationDate(token);

            // Assert
            expirationDate.Should().NotBeNull();
            expirationDate.Should().BeAfter(DateTime.UtcNow);
            expirationDate.Should().BeCloseTo(
                DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                TimeSpan.FromMinutes(1)
            );
        }

        [Theory]
        [InlineData(Roles.Patient)]
        [InlineData(Roles.Doctor)]
        [InlineData(Roles.HospitalAdmin)]
        [InlineData(Roles.SystemAdmin)]
        public void GenerateAccessToken_WithDifferentRoles_ShouldIncludeRoleInToken(string role)
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "Test User");
            var roles = new List<string> { role };

            // Act
            var token = _jwtService.GenerateAccessToken(user, roles);
            var principal = _jwtService.ValidateToken(token);

            // Assert
            principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be(role);
        }

        [Fact]
        public void GenerateAccessToken_WithMultipleRoles_ShouldIncludeAllRoles()
        {
            // Arrange
            var user = new User("adminuser", "admin@example.com", "Admin User");
            var roles = new List<string> { Roles.Doctor, Roles.HospitalAdmin };

            // Act
            var token = _jwtService.GenerateAccessToken(user, roles);
            var principal = _jwtService.ValidateToken(token);

            // Assert
            var roleClaims = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            roleClaims.Should().Contain(Roles.Doctor);
            roleClaims.Should().Contain(Roles.HospitalAdmin);
        }
    }
}