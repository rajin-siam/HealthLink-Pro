using HealthLink.API.Extensions;
using HealthLink.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Controllers
builder.Services.AddControllers();

// Database
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<HealthLinkDbContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("HealthLink.Data"))
           .UseLazyLoadingProxies()
);

// ⚠️ ADD THESE MISSING SERVICES:
builder.Services.AddIdentityServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddApplicationServices();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed roles
await ServiceExtensions.SeedRoles(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// ⚠️ ADD AUTHENTICATION BEFORE AUTHORIZATION:
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();