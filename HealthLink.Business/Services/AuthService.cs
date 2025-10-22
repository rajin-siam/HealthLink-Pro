using HealthLink.Core.Constants;
using HealthLink.Core.Entities;
using HealthLink.Core.Interfaces;
using HealthLink.Core.Models;
using HealthLink.Core.Models.Auth;
using HealthLink.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthLink.Business.Services
{
    /// <summary>
    /// Implementation of authentication and authorization operations.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly HealthLinkDbContext _context;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            ILogger<AuthService> logger,
            HealthLinkDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
            _context = context;
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(Core.Models.Auth.RegisterRequest request)
        {
            try
            {
                // Validate role
                if (!Roles.IsValidRole(request.Role))
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Invalid role specified.",
                        new List<string> { $"Role '{request.Role}' is not valid." }
                    );
                }

                // Create user
                var user = new User(request.UserName, request.Email, request.FullName);

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "User registration failed.",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                // Add role
                var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

                if (!roleResult.Succeeded)
                {
                    // Cleanup: delete user if role assignment fails
                    await _userManager.DeleteAsync(user);

                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Failed to assign role to user.",
                        roleResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                // Generate tokens
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Store refresh token
                var refreshTokenEntity = new RefreshToken(
                    user.Id,
                    refreshToken,
                    DateTime.UtcNow.AddDays(7),
                    "Registration"
                );
                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var authResponse = new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName,
                        Roles = roles.ToList(),
                        PatientId = user.PatientId,
                        DoctorId = user.DoctorId,
                        HospitalId = user.HospitalId
                    }
                };

                _logger.LogInformation("User {UserName} registered successfully with role {Role}",
                    user.UserName, request.Role);

                return ApiResponse<AuthResponse>.SuccessResponse(
                    authResponse,
                    "User registered successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return ApiResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during registration.",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(Core.Models.Auth.LoginRequest request, string ipAddress)
        {
            try
            {
                // Find user by username or email
                User user;
                if (request.UserNameOrEmail.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
                }

                if (user == null)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Invalid username/email or password.",
                        new List<string> { "User not found." }
                    );
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Account is inactive.",
                        new List<string> { "Your account has been deactivated. Please contact support." }
                    );
                }

                // Verify password
                var signInResult = await _signInManager.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    lockoutOnFailure: true
                );

                if (!signInResult.Succeeded)
                {
                    if (signInResult.IsLockedOut)
                    {
                        return ApiResponse<AuthResponse>.ErrorResponse(
                            "Account locked.",
                            new List<string> { "Account is locked due to multiple failed login attempts." }
                        );
                    }

                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Invalid username/email or password.",
                        new List<string> { "Authentication failed." }
                    );
                }

                // Update last login
                user.RecordLogin();
                await _userManager.UpdateAsync(user);

                // Generate tokens
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Store refresh token
                var refreshTokenEntity = new RefreshToken(
                    user.Id,
                    refreshToken,
                    DateTime.UtcNow.AddDays(7),
                    ipAddress
                );
                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var authResponse = new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName,
                        Roles = roles.ToList(),
                        PatientId = user.PatientId,
                        DoctorId = user.DoctorId,
                        HospitalId = user.HospitalId
                    }
                };

                _logger.LogInformation("User {UserName} logged in successfully from {IpAddress}",
                    user.UserName, ipAddress);

                return ApiResponse<AuthResponse>.SuccessResponse(
                    authResponse,
                    "Login successful."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return ApiResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during login.",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
        {
            try
            {
                var userId = _jwtService.GetUserIdFromToken(request.Token);
                if (userId == null)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Invalid token.",
                        new List<string> { "Token validation failed." }
                    );
                }

                // Validate refresh token
                var refreshTokenEntity = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.RefreshTokenValue == request.RefreshToken && rt.User.Id == userId);

                if (refreshTokenEntity == null)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Invalid refresh token.",
                        new List<string> { "Refresh token not found." }
                    );
                }

                if (!refreshTokenEntity.IsActive)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "Refresh token is not active.",
                        new List<string> { "Token has been revoked or used." }
                    );
                }

                // Mark old refresh token as used
               // refreshTokenEntity.MarkAsUsed();

                var user = await _userManager.FindByIdAsync(userId.Value.ToString());
                if (user == null || !user.IsActive)
                {
                    return ApiResponse<AuthResponse>.ErrorResponse(
                        "User not found or inactive.",
                        new List<string> { "Cannot refresh token." }
                    );
                }

                // Generate new tokens
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Store new refresh token
                var newRefreshTokenEntity = new RefreshToken(
                    user.Id,
                    newRefreshToken,
                    DateTime.UtcNow.AddDays(7),
                    ipAddress
                );
                _context.RefreshTokens.Add(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                var authResponse = new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName,
                        Roles = roles.ToList(),
                        PatientId = user.PatientId,
                        DoctorId = user.DoctorId,
                        HospitalId = user.HospitalId
                    }
                };

                _logger.LogInformation("Token refreshed for user {UserName}", user.UserName);

                return ApiResponse<AuthResponse>.SuccessResponse(
                    authResponse,
                    "Token refreshed successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh");
                return ApiResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during token refresh.",
                    new List<string> { ex.Message }
                );
            }
        }

        //public async Task<ApiResponse<bool>> RevokeTokenAsync(string token, string ipAddress)
        //{
        //    try
        //    {
        //        var refreshTokenEntity = await _context.RefreshTokens
        //            .FirstOrDefaultAsync(rt => rt.refreshToken == token);

        //        if (refreshTokenEntity == null)
        //        {
        //            return ApiResponse<bool>.ErrorResponse(
        //                "Invalid token.",
        //                new List<string> { "Token not found." }
        //            );
        //        }

        //        if (!refreshTokenEntity.IsActive)
        //        {
        //            return ApiResponse<bool>.ErrorResponse(
        //                "Token is not active.",
        //                new List<string> { "Token is already revoked or used." }
        //            );
        //        }

        //        refreshTokenEntity.Revoke(ipAddress);
        //        await _context.SaveChangesAsync();

        //        _logger.LogInformation("Token revoked for user {UserId} from {IpAddress}",
        //            refreshTokenEntity.UserId, ipAddress);

        //        return ApiResponse<bool>.SuccessResponse(true, "Token revoked successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred during token revocation");
        //        return ApiResponse<bool>.ErrorResponse(
        //            "An error occurred during token revocation.",
        //            new List<string> { ex.Message }
        //        );
        //    }
        //}

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(Core.Models.Auth.ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Don't reveal that the user doesn't exist
                    return ApiResponse<bool>.SuccessResponse(
                        true,
                        "If the email exists, a password reset link has been sent."
                    );
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // TODO: Send email with reset token
                // For now, just log it
                _logger.LogInformation("Password reset token for {Email}: {Token}", request.Email, token);
                _logger.LogInformation("Password reset requested for user {UserName}", user.UserName);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Password reset link has been sent to your email."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset request");
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during password reset.",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(Core.Models.Auth.ResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Invalid reset request.",
                        new List<string> { "User not found." }
                    );
                }

                var result = await _userManager.ResetPasswordAsync(
                    user,
                    request.Token,
                    request.NewPassword
                );

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Password reset failed.",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                _logger.LogInformation("Password reset successful for user {UserName}", user.UserName);

                return ApiResponse<bool>.SuccessResponse(true, "Password reset successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset");
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during password reset.",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "User not found.",
                        new List<string> { "Invalid user ID." }
                    );
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword
                );

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Password change failed.",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                _logger.LogInformation("Password changed for user {UserName}", user.UserName);

                return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password change");
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during password change.",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<UserInfo>> GetUserInfoAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return ApiResponse<UserInfo>.ErrorResponse(
                        "User not found.",
                        new List<string> { "Invalid user ID." }
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);

                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    PatientId = user.PatientId,
                    DoctorId = user.DoctorId,
                    HospitalId = user.HospitalId
                };

                return ApiResponse<UserInfo>.SuccessResponse(userInfo, "User info retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user info");
                return ApiResponse<UserInfo>.ErrorResponse(
                    "An error occurred while retrieving user info.",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ConfirmEmailAsync(Guid userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "User not found.",
                        new List<string> { "Invalid user ID." }
                    );
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Email confirmation failed.",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                _logger.LogInformation("Email confirmed for user {UserName}", user.UserName);

                return ApiResponse<bool>.SuccessResponse(true, "Email confirmed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during email confirmation");
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during email confirmation.",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}