
using HealthLink.Core.Models.Auth;
using HealthLink.Core.Models;

namespace HealthLink.Business.Services
{
    /// <summary>
    /// Implementation of authentication and authorization operations.
    /// </summary>
    public interface IAuthService
    {

        public Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        public Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress);
        public Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
        //        public Task<ApiResponse<bool>> RevokeTokenAsync(string token, string ipAddress);
        public Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        public Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        public Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        public Task<ApiResponse<UserInfo>> GetUserInfoAsync(Guid userId);
        public Task<ApiResponse<bool>> ConfirmEmailAsync(Guid userId, string token);
    }
}