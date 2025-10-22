using FluentAssertions;
using Microsoft.EntityFrameworkCore; // Add this using directive at the top of the file
using HealthLink.Business.Services;
using HealthLink.Core.Constants;
using HealthLink.Core.Entities;
using HealthLink.Core.Interfaces;
using HealthLink.Core.Models.Auth;
using HealthLink.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.InMemory;
namespace HealthLink.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly HealthLinkDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Mock UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null
            );

            // Mock SignInManager
            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                null, null, null, null
            );

            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            // Setup in-memory database
            var options = new DbContextOptionsBuilder<HealthLinkDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new HealthLinkDbContext(options);

            _authService = new AuthService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _jwtServiceMock.Object,
                _loggerMock.Object,
                _context
            );
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var request = new Core.Models.Auth.RegisterRequest
            {
                UserName = "johndoe",
                Email = "john@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Role = Roles.Patient
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { Roles.Patient });

            _jwtServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
                .Returns("test-jwt-token");

            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Token.Should().Be("test-jwt-token");
            result.Data.RefreshToken.Should().Be("test-refresh-token");
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidRole_ShouldFail()
        {
            // Arrange
            var request = new Core.Models.Auth.RegisterRequest
            {
                UserName = "johndoe",
                Email = "john@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Role = "InvalidRole"
            };

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid role");
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldFail()
        {
            // Arrange
            var request = new Core.Models.Auth.RegisterRequest
            {
                UserName = "johndoe",
                Email = "john@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Role = Roles.Patient
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Description = "Email already exists" }
                ));

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Email already exists");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldSucceed()
        {
            // Arrange
            var request = new Core.Models.Auth.LoginRequest
            {
                UserNameOrEmail = "johndoe",
                Password = "Password123!",
                RememberMe = false
            };

            var user = new User("johndoe", "john@example.com", "John Doe");

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { Roles.Patient });

            _jwtServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
                .Returns("test-jwt-token");

            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // Act
            var result = await _authService.LoginAsync(request, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Token.Should().Be("test-jwt-token");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldFail()
        {
            // Arrange
            var request = new Core.Models.Auth.LoginRequest
            {
                UserNameOrEmail = "johndoe",
                Password = "WrongPassword",
                RememberMe = false
            };

            var user = new User("johndoe", "john@example.com", "John Doe");

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _authService.LoginAsync(request, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid");
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ShouldFail()
        {
            // Arrange
            var request = new Core.Models.Auth.LoginRequest
            {
                UserNameOrEmail = "johndoe",
                Password = "Password123!",
                RememberMe = false
            };

            var user = new User("johndoe", "john@example.com", "John Doe");
            user.Deactivate();

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _authService.LoginAsync(request, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("inactive");
        }

        [Fact]
        public async Task LoginAsync_WithEmailAddress_ShouldSucceed()
        {
            // Arrange
            var request = new Core.Models.Auth.LoginRequest
            {
                UserNameOrEmail = "john@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            var user = new User("johndoe", "john@example.com", "John Doe");

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { Roles.Patient });

            _jwtServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
                .Returns("test-jwt-token");

            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // Act
            var result = await _authService.LoginAsync(request, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            var user = new User("johndoe", "john@example.com", "John Doe");

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ChangePasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.ChangePasswordAsync(userId, request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserInfoAsync_WithValidUserId_ShouldReturnUserInfo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("johndoe", "john@example.com", "John Doe");

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { Roles.Patient });

            // Act
            var result = await _authService.GetUserInfoAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.UserName.Should().Be("johndoe");
            result.Data.Email.Should().Be("john@example.com");
            result.Data.Roles.Should().Contain(Roles.Patient);
        }
    }
}