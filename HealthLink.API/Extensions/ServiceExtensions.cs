using HealthLink.Business.Services;
using HealthLink.Core.Configuration;
using HealthLink.Core.Constants;
using HealthLink.Core.Entities;
using HealthLink.Core.Interfaces;
using HealthLink.Core.Models.Auth;
using HealthLink.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HealthLink.API.Extensions
{
    /// <summary>
    /// Extension methods for configuring services in the application.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configures Identity services with custom options.
        /// </summary>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Sign in settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<HealthLinkDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        /// <summary>
        /// Configures JWT authentication.
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JWT settings are not configured properly.");
            }

            jwtSettings.Validate();

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Set to true in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                    ),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = System.Text.Json.JsonSerializer.Serialize(
                            new { message = "You are not authorized to access this resource." }
                        );

                        return context.Response.WriteAsync(result);
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// Configures authorization policies.
        /// </summary>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Require active user
                options.AddPolicy(Policies.RequireActiveUser, policy =>
                    policy.RequireClaim(CustomClaims.IsActive, "True"));

                // Role-based policies
                options.AddPolicy(Policies.RequirePatientRole, policy =>
                    policy.RequireRole(Roles.Patient));

                options.AddPolicy(Policies.RequireDoctorRole, policy =>
                    policy.RequireRole(Roles.Doctor));

                options.AddPolicy(Policies.RequireHospitalAdminRole, policy =>
                    policy.RequireRole(Roles.HospitalAdmin));

                options.AddPolicy(Policies.RequireSystemAdminRole, policy =>
                    policy.RequireRole(Roles.SystemAdmin));

                // Combined policies
                options.AddPolicy(Policies.RequireAdminRole, policy =>
                    policy.RequireRole(Roles.HospitalAdmin, Roles.SystemAdmin));

                options.AddPolicy(Policies.RequireMedicalStaff, policy =>
                    policy.RequireRole(Roles.Doctor, Roles.HospitalAdmin, Roles.SystemAdmin));
            });

            return services;
        }

        /// <summary>
        /// Registers application services.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register JWT service
            services.AddScoped<IJwtService, JwtService>();

            // Register Auth service (DbContext is already registered, will be injected automatically)
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        /// <summary>
        /// Seeds initial roles in the database.
        /// </summary>
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var roleName in Roles.GetAllRoles())
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }
    }
}