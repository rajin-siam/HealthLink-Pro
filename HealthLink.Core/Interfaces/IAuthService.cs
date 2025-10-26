using HealthLink.Core.Models.Auth;
using HealthLink.Core.Models;

namespace HealthLink.Business.Services
{
    /// <summary>
    /// Interface for authentication and authorization operations.
    /// Defines all authentication-related methods including registration, login, token management, and password operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user with specified role.
        /// </summary>
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Authenticates user with username/email and password.
        /// </summary>
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress);

        /// <summary>
        /// Generates new access token using valid refresh token.
        /// </summary>
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);

        /// <summary>
        /// Revokes a refresh token (for logout functionality).
        /// </summary>
        Task<ApiResponse<bool>> RevokeTokenAsync(string token, string ipAddress);

        /// <summary>
        /// Initiates password reset by generating reset token.
        /// </summary>
        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);

        /// <summary>
        /// Resets password using the reset token.
        /// </summary>
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// Changes password for authenticated user.
        /// </summary>
        Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);

        /// <summary>
        /// Gets information about a user.
        /// </summary>
        Task<ApiResponse<UserInfo>> GetUserInfoAsync(Guid userId);

        /// <summary>
        /// Confirms user email with confirmation token.
        /// </summary>
        Task<ApiResponse<bool>> ConfirmEmailAsync(Guid userId, string token);
    }
}