# HealthLink - Healthcare Management System
## Project Documentation

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Getting Started](#getting-started)
5. [Project Structure](#project-structure)
6. [Configuration](#configuration)
7. [Authentication & Security](#authentication--security)
8. [API Documentation](#api-documentation)
9. [Database Schema](#database-schema)
10. [Testing](#testing)
11. [Deployment](#deployment)

---

## Project Overview

### About HealthLink

**HealthLink** is a comprehensive healthcare management system built with ASP.NET Core 8.0 that enables efficient management of healthcare operations including patient records, doctor appointments, medical records, and prescription management.

### Key Features

- ✅ **User Management**: Role-based authentication and authorization (Patient, Doctor, Hospital Admin, System Admin)
- ✅ **Patient Management**: Complete patient health records with allergies, demographics, and medical history
- ✅ **Appointment System**: Scheduling, rescheduling, and status tracking of patient-doctor appointments
- ✅ **Medical Records**: Comprehensive medical record creation with diagnosis, symptoms, and treatment plans
- ✅ **Prescription Management**: Digital prescription creation and tracking with medication details
- ✅ **Hospital Management**: Multi-hospital support with doctor assignments
- ✅ **Security**: JWT-based authentication with refresh token support
- ✅ **Audit Trail**: Automatic tracking of creation and modification timestamps

### System Goals

1. **Efficiency**: Streamline healthcare workflows and reduce administrative overhead
2. **Security**: Protect sensitive patient data with robust authentication and authorization
3. **Scalability**: Support multiple hospitals, doctors, and thousands of patients
4. **Maintainability**: Clean architecture for easy updates and feature additions
5. **Reliability**: Comprehensive testing and error handling

---

## System Architecture

### Architecture Pattern

HealthLink follows **Clean Architecture** principles with clear separation of concerns across four layers:

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│                    (HealthLink.API)                         │
│  • REST API Controllers                                     │
│  • Request/Response Models                                  │
│  • Authentication Middleware                                │
│  • Dependency Injection Configuration                       │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                     Business Layer                           │
│                  (HealthLink.Business)                      │
│  • Service Implementations (AuthService, JwtService)        │
│  • Business Logic Orchestration                             │
│  • Transaction Management                                   │
│  • Validation & Error Handling                              │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                      Core Layer                              │
│                   (HealthLink.Core)                         │
│  • Domain Entities (Patient, Doctor, Appointment, etc.)     │
│  • Business Rules & Validations                             │
│  • Interfaces & Contracts                                   │
│  • Enums, Constants, DTOs                                   │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                    Infrastructure Layer                      │
│                    (HealthLink.Data)                        │
│  • Entity Framework DbContext                               │
│  • Database Migrations                                      │
│  • Entity Configurations                                    │
│  • Repository Implementations (if needed)                   │
└─────────────────────────────────────────────────────────────┘
```

### Design Principles

1. **Dependency Inversion**: High-level modules don't depend on low-level modules
2. **Single Responsibility**: Each class has one reason to change
3. **Open/Closed**: Open for extension, closed for modification
4. **Interface Segregation**: Clients don't depend on interfaces they don't use
5. **DRY (Don't Repeat Yourself)**: Reusable components and shared logic

---

## Technology Stack

### Core Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET Core | 8.0 | Application framework |
| C# | 12.0 | Programming language |
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM for database access |
| PostgreSQL | Latest | Relational database |
| ASP.NET Core Identity | 8.0 | User authentication |
| JWT | 8.14.0 | Token-based authentication |

### Development Tools

- **IDE**: Visual Studio 2022 / Visual Studio Code / Rider
- **API Testing**: Swagger UI, Postman
- **Database Management**: pgAdmin, DBeaver
- **Version Control**: Git
- **Testing**: xUnit, Moq, FluentAssertions

### NuGet Packages

**HealthLink.API:**
- `Swashbuckle.AspNetCore` (9.0.6) - Swagger/OpenAPI
- `Serilog.AspNetCore` (9.0.0) - Logging
- `Microsoft.EntityFrameworkCore.Design` (8.0.21)

**HealthLink.Business:**
- `FluentValidation` (12.0.0) - Input validation
- `Mapster` (7.4.0) - Object mapping
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.21)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.21)

**HealthLink.Data:**
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.11)
- `Microsoft.EntityFrameworkCore.Proxies` (8.0.21) - Lazy loading
- `Microsoft.EntityFrameworkCore.Tools` (8.0.21) - Migrations

**HealthLink.Tests:**
- `xUnit` (2.9.3) - Test framework
- `Moq` (4.20.72) - Mocking framework
- `FluentAssertions` (8.7.1) - Assertion library
- `Microsoft.EntityFrameworkCore.InMemory` (9.0.10) - In-memory database for testing

---

## Getting Started

### Prerequisites

1. **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **PostgreSQL** - [Download](https://www.postgresql.org/download/)
3. **IDE** (Visual Studio, VS Code, or Rider)
4. **Git** (optional but recommended)

### Installation Steps

#### 1. Clone the Repository

```bash
git clone https://github.com/your-org/healthlink.git
cd healthlink
```

#### 2. Configure Database Connection

Create `appsettings.Development.json` in the `HealthLink.API` project (this file is gitignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=healthlink_db;Username=postgres;Password=your_password;"
  },
  "JwtSettings": {
    "SecretKey": "YourVerySecureSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "HealthLinkAPI",
    "Audience": "HealthLinkClient",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

#### 3. Create Database

```bash
# From solution root directory
dotnet ef database update --project HealthLink.Data --startup-project HealthLink.API
```

This will:
- Create the `healthlink_db` database
- Apply all migrations
- Create all tables and relationships

#### 4. Run the Application

```bash
cd HealthLink.API
dotnet run
```

Or use Visual Studio's IIS Express/Kestrel profile.

#### 5. Access the Application

- **API Base URL**: `https://localhost:5001` or `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`

---

## Project Structure

### Solution Layout

```
HealthLink/
├── HealthLink.API/                 # Presentation Layer
│   ├── Controllers/
│   │   └── AuthController.cs      # Authentication endpoints
│   ├── Extensions/
│   │   └── ServiceExtensions.cs   # DI configuration
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs                 # Application entry point
│   └── appsettings.json
│
├── HealthLink.Business/           # Business Logic Layer
│   ├── Services/
│   │   ├── AuthService.cs        # Authentication logic
│   │   └── JwtService.cs         # JWT token management
│   └── HealthLink.Business.csproj
│
├── HealthLink.Core/               # Domain Layer
│   ├── Configuration/
│   │   └── JwtSettings.cs        # JWT configuration model
│   ├── Constants/
│   │   ├── CustomClaims.cs       # JWT claim types
│   │   ├── Policies.cs           # Authorization policies
│   │   └── Roles.cs              # System roles
│   ├── Entities/                 # Domain models
│   │   ├── Allergy.cs
│   │   ├── Appointment.cs
│   │   ├── BaseEntity.cs
│   │   ├── Doctor.cs
│   │   ├── Hospital.cs
│   │   ├── MedicalRecord.cs
│   │   ├── Patient.cs
│   │   ├── Prescription.cs
│   │   ├── RefreshToken.cs
│   │   └── User.cs
│   ├── Enums/
│   │   ├── AllergySeverity.cs
│   │   ├── AppointmentStatus.cs
│   │   └── Specialization.cs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── IJwtService.cs
│   ├── Models/
│   │   ├── ApiResponse.cs
│   │   └── Auth/                 # Auth DTOs
│   ├── Validators/
│   │   └── PatientValidator.cs
│   └── HealthLink.Core.csproj
│
├── HealthLink.Data/               # Infrastructure Layer
│   ├── Context/
│   │   └── HealthLinkDbContext.cs # EF Core DbContext
│   ├── Migrations/               # EF Core migrations
│   └── HealthLink.Data.csproj
│
├── HealthLink.Tests/              # Test Project
│   ├── Entities/                 # Entity tests
│   │   ├── PatientTests.cs
│   │   ├── DoctorTests.cs
│   │   ├── AppointmentTests.cs
│   │   ├── MedicalRecordTests.cs
│   │   └── UserTests.cs
│   ├── Services/                 # Service tests
│   │   ├── AuthServiceTests.cs
│   │   └── JwtServiceTests.cs
│   └── HealthLink.Tests.csproj
│
├── .gitignore
├── HealthLink.sln
└── README.md
```

### Key Files Explained

**Program.cs**: Application startup and middleware configuration
**ServiceExtensions.cs**: Dependency injection and service registration
**HealthLinkDbContext.cs**: Database context with entity configurations
**BaseEntity.cs**: Base class for all entities with audit properties
**ApiResponse.cs**: Standardized API response wrapper

---

## Configuration

### Application Settings

#### appsettings.json (Committed)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=healthlink_db;Username=postgres;Password=iamdragon;"
  },
  "JwtSettings": {
    "SecretKey": "YourVerySecureSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "HealthLinkAPI",
    "Audience": "HealthLinkClient",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

#### appsettings.Development.json (Gitignored)

Override settings for local development:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=healthlink_dev;Username=dev_user;Password=dev_pass;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Environment Variables (Production)

For production, use environment variables instead of configuration files:

```bash
export DATABASE_URL="Host=prod-db;Database=healthlink;Username=app;Password=***"
export JWT_SECRET_KEY="ProductionSecretKey..."
export ASPNETCORE_ENVIRONMENT="Production"
```

### Identity Configuration

Located in `ServiceExtensions.cs`:

```csharp
// Password requirements
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 6;
options.Password.RequiredUniqueChars = 1;

// Account lockout
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;

// User settings
options.User.RequireUniqueEmail = true;
options.SignIn.RequireConfirmedEmail = false; // Set to true for production
```

---

## Authentication & Security

### JWT Authentication Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     Authentication Flow                      │
└─────────────────────────────────────────────────────────────┘

1. User Login
   ↓
2. Validate Credentials
   ↓
3. Generate Access Token (60 min expiry)
   ↓
4. Generate Refresh Token (7 day expiry)
   ↓
5. Store Refresh Token in Database
   ↓
6. Return Both Tokens to Client

──────────────────────────────────────────────────────────────

Client makes API request:
   Headers: { Authorization: "Bearer <access_token>" }
   ↓
Middleware validates token signature & expiry
   ↓
If valid: Extract user claims → Allow request
If expired: Return 401 → Client uses refresh token
```

### Token Types

**Access Token**:
- Short-lived (60 minutes)
- Contains user claims (ID, email, roles)
- Stateless - validated via signature
- Used for API authentication

**Refresh Token**:
- Long-lived (7 days)
- Stored in database
- Used to obtain new access tokens
- Can be revoked for logout

### Security Features

1. **Password Security**:
   - Hashed using PBKDF2 (ASP.NET Identity default)
   - Minimum 6 characters with complexity requirements
   - Never stored in plain text

2. **Account Lockout**:
   - 5 failed attempts → 15-minute lockout
   - Prevents brute force attacks

3. **Role-Based Authorization**:
   - Patient, Doctor, HospitalAdmin, SystemAdmin
   - Custom policies for fine-grained access control

4. **Audit Trail**:
   - All entities track CreatedDate and UpdatedDate
   - Medical records track creator and modifier doctors

### Authorization Policies

Defined in `ServiceExtensions.cs`:

```csharp
// Require active account
options.AddPolicy(Policies.RequireActiveUser, policy =>
    policy.RequireClaim(CustomClaims.IsActive, "True"));

// Role-based policies
options.AddPolicy(Policies.RequirePatientRole, policy =>
    policy.RequireRole(Roles.Patient));

options.AddPolicy(Policies.RequireDoctorRole, policy =>
    policy.RequireRole(Roles.Doctor));

// Combined policies
options.AddPolicy(Policies.RequireMedicalStaff, policy =>
    policy.RequireRole(Roles.Doctor, Roles.HospitalAdmin, Roles.SystemAdmin));
```

---

## API Documentation

### Base URL

- **Development**: `https://localhost:5001/api`
- **Production**: `https://your-domain.com/api`

### Authentication Endpoints

#### POST /api/auth/register
Register a new user account.

**Request Body:**
```json
{
  "userName": "johndoe",
  "email": "john@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "fullName": "John Doe",
  "role": "Patient"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "User registered successfully.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiresAt": "2025-10-28T10:30:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "johndoe",
      "email": "john@example.com",
      "fullName": "John Doe",
      "roles": ["Patient"]
    }
  },
  "errors": []
}
```

#### POST /api/auth/login
Authenticate an existing user.

**Request Body:**
```json
{
  "userNameOrEmail": "john@example.com",
  "password": "Password123!",
  "rememberMe": false
}
```

**Response (200 OK):** Same as register response

#### POST /api/auth/refresh-token
Obtain a new access token using refresh token.

**Request Body:**
```json
{
  "token": "expired-access-token",
  "refreshToken": "valid-refresh-token"
}
```

**Response (200 OK):** Returns new access and refresh tokens

#### POST /api/auth/revoke-token
Revoke a refresh token (logout).

**Authorization:** Required  
**Request Body:**
```json
{
  "refreshToken": "token-to-revoke"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Token revoked successfully.",
  "data": true,
  "errors": []
}
```

#### GET /api/auth/me
Get current authenticated user information.

**Authorization:** Required  
**Response (200 OK):**
```json
{
  "success": true,
  "message": "User info retrieved successfully.",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "johndoe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "roles": ["Patient"],
    "patientId": "guid",
    "doctorId": null,
    "hospitalId": null
  },
  "errors": []
}
```

### Error Responses

**400 Bad Request:**
```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": [
    "Password is required",
    "Email must be valid"
  ]
}
```

**401 Unauthorized:**
```json
{
  "success": false,
  "message": "Invalid username/email or password.",
  "data": null,
  "errors": ["Authentication failed."]
}
```

### Using Swagger UI

1. Navigate to `https://localhost:5001/swagger`
2. Click "Authorize" button at top right
3. Enter JWT token: `Bearer your-access-token-here`
4. Test endpoints interactively

---

## Database Schema

### Entity Relationship Diagram

```
┌─────────────┐
│   Hospital  │
│─────────────│
│ Id (PK)     │
│ Name        │
│ RegNumber   │
└──────┬──────┘
       │ 1
       │
       │ N
┌──────▼──────┐         ┌─────────────┐
│   Doctor    │         │    User     │
│─────────────│         │─────────────│
│ Id (PK)     │◄────────│ Id (PK)     │
│ Name        │   0..1  │ UserName    │
│ LicenseNum  │         │ Email       │
│ HospitalId  │         │ DoctorId    │
└──────┬──────┘         │ PatientId   │
       │                └──────┬──────┘
       │ N                     │ 0..1
       │                       │
       │              ┌────────▼─────────┐
       │              │     Patient      │
       │              │──────────────────│
       │              │ Id (PK)          │
       │              │ Name             │
       │              │ Email            │
       │              │ BloodType        │
       │              └────┬─────┬───────┘
       │                   │ 1   │ 1
       │ 1                 │ N   │ N
       │              ┌────▼─────▼───────┐
       │              │    Allergy       │
       │              │  Appointment     │
       ├──────────────┤  MedicalRecord   │
       │              └──────────────────┘
       │ 1
       │ N
┌──────▼──────────────┐
│   MedicalRecord     │
│─────────────────────│
│ Id (PK)             │
│ PatientId (FK)      │
│ CreatedByDoctorId   │
│ ModifiedByDoctorId  │
│ Diagnosis           │
│ Symptoms            │
└──────┬──────────────┘
       │ 1
       │ N
┌──────▼──────────────┐
│   Prescription      │
│─────────────────────│
│ Id (PK)             │
│ MedicalRecordId(FK) │
│ PrescribedByDoc(FK) │
│ MedicationName      │
│ Dosage              │
└─────────────────────┘
```

### Core Tables

**Users** (Identity table)
- Stores authentication data
- Links to domain entities (Patient, Doctor, Hospital)

**Patients**
- Personal information and health metrics
- One-to-many with Allergies, Appointments, MedicalRecords

**Doctors**
- Professional information and credentials
- Belongs to a Hospital
- Creates MedicalRecords and Prescriptions

**Hospitals**
- Healthcare facility information
- Has many Doctors

**Appointments**
- Links Patient and Doctor
- Tracks appointment status and details

**MedicalRecords**
- Patient's health record for a visit
- Created by one Doctor, modified by another (optional)
- Contains multiple Prescriptions

**Prescriptions**
- Medication information
- Belongs to MedicalRecord
- Prescribed by Doctor

**Allergies**
- Patient's allergy information
- Severity levels and reactions

**RefreshTokens**
- JWT refresh tokens
- Used for token refresh flow

---

## Testing

### Test Structure

```
HealthLink.Tests/
├── Entities/              # Domain entity tests
│   ├── PatientTests.cs
│   ├── DoctorTests.cs
│   ├── AppointmentTests.cs
│   ├── MedicalRecordTests.cs
│   └── UserTests.cs
└── Services/              # Service layer tests
    ├── AuthServiceTests.cs
    └── JwtServiceTests.cs
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage (if configured)
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "FullyQualifiedName~PatientTests"
```

### Test Examples

**Entity Test:**
```csharp
[Fact]
public void CreatePatient_WithValidData_ShouldSucceed()
{
    // Arrange
    var patient = new Patient(
        id: Guid.NewGuid(),
        name: "John Doe",
        email: "john@example.com",
        bloodType: "O+",
        height: 180,
        weight: 75
    );

    // Act & Assert
    Assert.Equal("John Doe", patient.Name);
    Assert.NotEmpty(patient.Allergies);
}
```

**Service Test with Mocking:**
```csharp
[Fact]
public async Task RegisterAsync_WithValidData_ShouldSucceed()
{
    // Arrange
    var request = new RegisterRequest { ... };
    _userManagerMock.Setup(x => x.CreateAsync(...))
        .ReturnsAsync(IdentityResult.Success);

    // Act
    var result = await _authService.RegisterAsync(request);

    // Assert
    result.Success.Should().BeTrue();
    result.Data.Should().NotBeNull();
}
```

### Testing Best Practices

1. **AAA Pattern**: Arrange, Act, Assert
2. **One assertion per test** (when possible)
3. **Descriptive test names**: `Method_Scenario_ExpectedResult`
4. **Mock external dependencies**
5. **Use in-memory database for integration tests**

---

## Deployment

### Prerequisites

1. Production PostgreSQL database
2. Server or cloud hosting (Azure, AWS, etc.)
3. SSL certificate for HTTPS
4. Environment variable management

### Deployment Checklist

#### 1. Update Configuration

- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure production connection string
- [ ] Use secure JWT secret key (environment variable)
- [ ] Enable HTTPS redirection
- [ ] Disable Swagger UI
- [ ] Configure CORS for specific origins
- [ ] Set up proper logging (Serilog to cloud)

#### 2. Database Migration

```bash
# Generate SQL script from migrations
dotnet ef migrations script --project HealthLink.Data \
  --startup-project HealthLink.API \
  --output migrate.sql

# Apply to production database
psql -h prod-host -U app_user -d healthlink < migrate.sql
```

#### 3. Build and Publish

```bash
# Publish release build
dotnet publish HealthLink.API/HealthLink.API.csproj \
  -c Release \
  -o ./publish

# Copy published files to server
scp -r ./publish/* user@server:/var/www/healthlink/
```

#### 4. Configure Systemd Service (Linux)

Create `/etc/systemd/system/healthlink.service`:

```ini
[Unit]
Description=HealthLink API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/healthlink
ExecStart=/usr/bin/dotnet /var/www/healthlink/HealthLink.API.dll
Restart=always
RestartSec=10
SyslogIdentifier=healthlink
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DATABASE_URL=<connection-string>
Environment=JWT_SECRET_KEY=<secret-key>

[Install]
WantedBy=multi-user.target
```

Enable and start:
```bash
sudo systemctl enable healthlink
sudo systemctl start healthlink
sudo systemctl status healthlink
```

#### 5. Configure Nginx (Reverse Proxy)

```nginx
server {
    listen 80;
    server_name api.healthlink.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api.healthlink.com;

    ssl_certificate /etc/ssl/certs/healthlink.crt;
    ssl_certificate_key /etc/ssl/private/healthlink.key;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Docker Deployment (Optional)

**Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HealthLink.API/HealthLink.API.csproj", "HealthLink.API/"]
COPY ["HealthLink.Business/HealthLink.Business.csproj", "HealthLink.Business/"]
COPY ["HealthLink.Core/HealthLink.Core.csproj", "HealthLink.Core/"]
COPY ["HealthLink.Data/HealthLink.Data.csproj", "HealthLink.Data/"]
RUN dotnet restore "HealthLink.API/HealthLink.API.csproj"
COPY . .
WORKDIR "/src/HealthLink.API"
RUN dotnet build "HealthLink.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthLink.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthLink.API.dll"]
```

**docker-compose.yml:**
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DATABASE_URL=${DATABASE_URL}
      - JWT_SECRET_KEY=${JWT_SECRET_KEY}
    depends_on:
      - db
  
  db:
    image: postgres:latest
    environment:
      POSTGRES_DB: healthlink
      POSTGRES_USER: app
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

---

## Troubleshooting

### Common Issues

**Issue: Database connection fails**
```
Solution: Verify PostgreSQL is running and connection string is correct
Check: Connection string, firewall rules, PostgreSQL service status
```

**Issue: Migrations fail**
```
Solution: Ensure HealthLink.Data is specified as project parameter
Command: dotnet ef database update --project HealthLink.Data --startup-project HealthLink.API
```

**Issue: JWT token validation fails**
```
Solution: Check JWT secret key matches in configuration and token generation
Verify: Token signature, expiration time, issuer/audience claims
```

**Issue: CORS errors in browser**
```
Solution: Configure CORS policy in Program.cs
Check: AllowedOrigins, AllowedMethods, AllowedHeaders settings
```

**Issue: Entity Framework lazy loading not working**
```
Solution: Ensure UseLazyLoadingProxies() is called and navigation properties are virtual
Check: DbContext configuration, entity property declarations
```

### Debugging Tips

1. **Enable detailed logging**:
```csharp
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft.EntityFrameworkCore": "Information"
  }
}
```

2. **Check database queries**:
```csharp
// Add to Program.cs for development
if (app.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<HealthLinkDbContext>(options =>
        options.UseNpgsql(connectionString)
               .LogTo(Console.WriteLine, LogLevel.Information));
}
```

3. **Test API endpoints with curl**:
```bash
# Test registration
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"userName":"test","email":"test@test.com","password":"Test123!","confirmPassword":"Test123!","fullName":"Test User","role":"Patient"}'

# Test authenticated endpoint
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer your-token-here"
```

---

## Maintenance & Support

### Regular Maintenance Tasks

**Daily:**
- Monitor application logs for errors
- Check database connections and performance
- Review authentication failures

**Weekly:**
- Review security logs
- Check disk space and database size
- Update dependencies if needed

**Monthly:**
- Database backup and restoration test
- Security audit and vulnerability scan
- Performance optimization review

### Backup Strategy

**Database Backups:**
```bash
# Daily automated backup
pg_dump -h localhost -U postgres healthlink_db > backup_$(date +%Y%m%d).sql

# Restore from backup
psql -h localhost -U postgres healthlink_db < backup_20251028.sql
```

**Application Files:**
- Source code in version control (Git)
- Configuration files backed up securely
- SSL certificates stored safely

---

## Contributing Guidelines

### Code Style

- Follow C# naming conventions (PascalCase for public members)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose
- Maximum line length: 120 characters

### Git Workflow

1. Create feature branch from `develop`
```bash
git checkout -b feature/new-feature-name
```

2. Make changes and commit with descriptive messages
```bash
git commit -m "Add: Patient allergy management endpoint"
```

3. Write tests for new features
4. Ensure all tests pass
5. Create pull request to `develop`
6. Code review and approval required
7. Merge to `develop`, then to `main` for release

### Commit Message Format

```
Type: Brief description

Detailed description (optional)

Type can be:
- Add: New feature
- Fix: Bug fix
- Update: Modification to existing feature
- Refactor: Code restructuring
- Test: Adding or updating tests
- Docs: Documentation changes
```

---

## Support & Resources

### Documentation
- **API Reference**: `/swagger` endpoint in development
- **Architecture Diagram**: See System Architecture section
- **Entity Models**: `HealthLink.Core/Entities/`

### External Resources
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [JWT Introduction](https://jwt.io/introduction)

### Contact
- **Project Lead**: [Name/Email]
- **Technical Support**: [Email]
- **Bug Reports**: [GitHub Issues URL]

---

## Appendix

### A. System Requirements

**Minimum:**
- CPU: 2 cores
- RAM: 4GB
- Storage: 20GB
- OS: Windows Server 2019+ / Linux (Ubuntu 20.04+)

**Recommended:**
- CPU: 4+ cores
- RAM: 8GB+
- Storage: 50GB+ SSD
- OS: Ubuntu 22.04 LTS or Windows Server 2022

### B. Port Configuration

| Port | Service | Purpose |
|------|---------|---------|
| 5000 | HTTP | Development API (insecure) |
| 5001 | HTTPS | Development API (secure) |
| 5432 | PostgreSQL | Database connection |
| 80 | HTTP | Production (redirects to 443) |
| 443 | HTTPS | Production API |

### C. Environment Variables Reference

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Yes | Application environment | `Production` |
| `DATABASE_URL` | Yes | PostgreSQL connection string | `Host=...;Database=...` |
| `JWT_SECRET_KEY` | Yes | JWT signing key | `32+ character string` |
| `JWT_ISSUER` | No | Token issuer | `HealthLinkAPI` |
| `JWT_AUDIENCE` | No | Token audience | `HealthLinkClient` |

### D. Database Indexes

Key indexes for performance:

```sql
-- Users
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);

-- Patients
CREATE UNIQUE INDEX IX_Patients_Email ON Patients(Email);
CREATE INDEX IX_Patients_CreatedDate ON Patients(CreatedDate);

-- Doctors
CREATE UNIQUE INDEX IX_Doctors_LicenseNumber ON Doctors(LicenseNumber);
CREATE INDEX IX_Doctors_Specialization ON Doctors(Specialization);

-- Appointments
CREATE INDEX IX_Appointments_AppointmentDateTime ON Appointments(AppointmentDateTime);
CREATE INDEX IX_Appointments_Status ON Appointments(Status);

-- RefreshTokens
CREATE UNIQUE INDEX IX_RefreshTokens_Token ON RefreshTokens(RefreshTokenValue);
CREATE INDEX IX_RefreshTokens_Active_Expiry ON RefreshTokens(IsActive, ExpiryDate);
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2025-01-15 | Initial release with core features |
| 1.1.0 | TBD | Planned: Doctor scheduling, notifications |

---

## License

[Specify your license here - MIT, Apache 2.0, proprietary, etc.]

---

**Document Version:** 1.0  
**Last Updated:** October 28, 2025  
**Maintained By:** HealthLink Development Team