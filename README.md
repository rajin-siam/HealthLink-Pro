# HealthLink System - Complete Documentation

## Table of Contents
1. [System Overview](#system-overview)
2. [Architecture](#architecture)
3. [Domain Model](#domain-model)
4. [Application Flow](#application-flow)
5. [Business Logic](#business-logic)
6. [Authentication & Authorization](#authentication--authorization)
7. [API Endpoints](#api-endpoints)
8. [Database Schema](#database-schema)

---

## System Overview

**HealthLink** is a comprehensive healthcare management system built with ASP.NET Core 8.0 that manages the relationships between patients, doctors, hospitals, appointments, medical records, and prescriptions.

### Key Features
- User authentication and role-based authorization (JWT)
- Patient health records management
- Doctor-patient appointment scheduling
- Medical record creation and tracking
- Prescription management
- Allergy tracking
- Hospital administration

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: PostgreSQL (via Npgsql)
- **Authentication**: ASP.NET Core Identity + JWT
- **Testing**: xUnit, Moq, FluentAssertions
- **Architecture**: Clean Architecture (Layered)

---

## Architecture

The application follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│                   (HealthLink.API)                       │
│  - Controllers                                           │
│  - Middleware                                            │
│  - Service Registration                                  │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                   Business Layer                         │
│                (HealthLink.Business)                     │
│  - AuthService (Authentication logic)                    │
│  - JwtService (Token generation/validation)              │
│  - Business Logic Implementation                         │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                     Core Layer                           │
│                  (HealthLink.Core)                       │
│  - Entities (Domain Models)                              │
│  - Interfaces                                            │
│  - Enums, Constants                                      │
│  - DTOs, Validators                                      │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                   Data Layer                             │
│                 (HealthLink.Data)                        │
│  - DbContext                                             │
│  - Entity Configurations                                 │
│  - Migrations                                            │
└──────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

**1. HealthLink.API (Presentation)**
- Exposes RESTful API endpoints
- Handles HTTP requests/responses
- Performs input validation
- Manages authentication/authorization
- Returns standardized API responses

**2. HealthLink.Business (Business Logic)**
- Implements business rules and workflows
- Orchestrates operations between entities
- Handles complex business scenarios
- Manages transactions

**3. HealthLink.Core (Domain)**
- Contains domain entities with business rules
- Defines interfaces and contracts
- Houses validation logic
- No dependencies on other layers

**4. HealthLink.Data (Infrastructure)**
- Manages database access via EF Core
- Configures entity relationships
- Handles migrations
- Implements repository pattern (if needed)

---

## Domain Model

### Core Entities

#### 1. **User** (Authentication Entity)
- Extends `IdentityUser<Guid>`
- Links to domain entities (Patient, Doctor, Hospital)
- Manages authentication state
- Tracks login activity

```
User
├── Id: Guid
├── UserName: string
├── Email: string
├── FullName: string
├── IsActive: bool
├── LastLoginDate: DateTime?
├── PatientId: Guid? ────┐
├── DoctorId: Guid? ─────┤
└── HospitalId: Guid? ───┤
                         │
        ┌────────────────┴────────────────┐
        │                                  │
    Patient                            Doctor
```

#### 2. **Patient**
- Core entity representing a patient
- Owns allergies, medical records, appointments
- Contains health metrics (height, weight, blood type)

#### 3. **Doctor**
- Represents medical professionals
- Has specialization and years of experience
- Creates medical records and prescriptions
- Schedules appointments
- Belongs to a hospital

#### 4. **Hospital**
- Healthcare facility
- Employs multiple doctors
- Contains contact and location information

#### 5. **Appointment**
- Connects patient with doctor
- Has status tracking (Scheduled, Completed, Cancelled, NoShow)
- Contains appointment details and notes

#### 6. **MedicalRecord**
- Patient's health record for a specific visit
- Created by a doctor
- Contains diagnosis, symptoms, treatment
- Can be modified by another doctor
- Contains multiple prescriptions

#### 7. **Prescription**
- Medication information
- Belongs to a medical record
- Prescribed by a doctor
- Contains dosage, frequency, duration

#### 8. **Allergy**
- Patient's allergy information
- Has severity levels (Mild, Moderate, Severe)
- Tracks identified date and reactions

#### 9. **RefreshToken**
- Manages JWT refresh tokens
- Tracks token validity and IP address
- Supports token refresh flow

### Entity Relationships

```
Hospital (1) ────── (N) Doctor
                           │
                           │ (N)
                           │
Patient (1) ────── (N) Appointment (N) ────── (1) Doctor
    │                                               │
    │ (1)                                          │ (N)
    │                                               │
    │                                          MedicalRecord
    │                                               │
    │ (N)                                          │ (1)
    │                                               │
Allergy                                       Prescription
    │                                               │
    └───────────────────────────────────────────────┘
                        (Many-to-One)
```

---

## Application Flow

### 1. User Registration Flow

```
┌─────────┐         ┌──────────────┐         ┌─────────────┐
│ Client  │         │AuthController│         │ AuthService │
└────┬────┘         └──────┬───────┘         └──────┬──────┘
     │                     │                        │
     │ POST /auth/register │                        │
     ├────────────────────>│                        │
     │                     │                        │
     │                     │ RegisterAsync()        │
     │                     ├───────────────────────>│
     │                     │                        │
     │                     │                   ┌────▼─────┐
     │                     │                   │ Validate │
     │                     │                   │   Role   │
     │                     │                   └────┬─────┘
     │                     │                        │
     │                     │                   ┌────▼─────────┐
     │                     │                   │ Create User  │
     │                     │                   │ via Identity │
     │                     │                   └────┬─────────┘
     │                     │                        │
     │                     │                   ┌────▼─────────┐
     │                     │                   │  Assign Role │
     │                     │                   └────┬─────────┘
     │                     │                        │
     │                     │                   ┌────▼─────────┐
     │                     │                   │  Generate    │
     │                     │                   │  JWT Tokens  │
     │                     │                   └────┬─────────┘
     │                     │                        │
     │                     │   AuthResponse         │
     │                     │<───────────────────────┤
     │   200 OK + Tokens   │                        │
     │<────────────────────┤                        │
     │                     │                        │
```

**Steps:**
1. Client sends registration request with username, email, password, role
2. Controller validates model state
3. AuthService validates role against allowed roles
4. Creates user via ASP.NET Identity UserManager
5. Assigns role to user
6. Generates JWT access token and refresh token
7. Stores refresh token in database
8. Returns tokens and user info to client

### 2. User Login Flow

```
┌─────────┐         ┌──────────────┐         ┌─────────────┐
│ Client  │         │AuthController│         │ AuthService │
└────┬────┘         └──────┬───────┘         └──────┬──────┘
     │                     │                        │
     │ POST /auth/login    │                        │
     ├────────────────────>│                        │
     │                     │                        │
     │                     │ LoginAsync()           │
     │                     ├───────────────────────>│
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │ Find User by  │
     │                     │                   │ Email/Username│
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │ Check if User │
     │                     │                   │   is Active   │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │    Verify     │
     │                     │                   │   Password    │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │ Update Last   │
     │                     │                   │  Login Date   │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │  Generate     │
     │                     │                   │  JWT Tokens   │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │   AuthResponse         │
     │                     │<───────────────────────┤
     │   200 OK + Tokens   │                        │
     │<────────────────────┤                        │
     │                     │                        │
```

**Steps:**
1. Client sends login credentials (username/email + password)
2. AuthService finds user by username or email
3. Checks if user account is active
4. Verifies password using SignInManager
5. Handles lockout if too many failed attempts
6. Records login timestamp
7. Generates new JWT tokens
8. Stores refresh token in database
9. Returns tokens and user info

### 3. Token Refresh Flow

```
┌─────────┐         ┌──────────────┐         ┌─────────────┐
│ Client  │         │AuthController│         │ AuthService │
└────┬────┘         └──────┬───────┘         └──────┬──────┘
     │                     │                        │
     │ POST /auth/         │                        │
     │ refresh-token       │                        │
     ├────────────────────>│                        │
     │                     │                        │
     │                     │ RefreshTokenAsync()    │
     │                     ├───────────────────────>│
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │ Extract UserId│
     │                     │                   │  from Token   │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │   Validate    │
     │                     │                   │ Refresh Token │
     │                     │                   │  from DB      │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │ Check if User │
     │                     │                   │   is Active   │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │                   ┌────▼──────────┐
     │                     │                   │  Generate New │
     │                     │                   │  JWT Tokens   │
     │                     │                   └────┬──────────┘
     │                     │                        │
     │                     │   New Tokens           │
     │                     │<───────────────────────┤
     │   200 OK + Tokens   │                        │
     │<────────────────────┤                        │
     │                     │                        │
```

### 4. Authenticated Request Flow

```
┌─────────┐    ┌──────────────┐    ┌─────────────┐    ┌─────────────┐
│ Client  │    │   JWT        │    │  Controller │    │   Service   │
│         │    │ Middleware   │    │             │    │             │
└────┬────┘    └──────┬───────┘    └──────┬──────┘    └──────┬──────┘
     │                │                   │                   │
     │ Request with   │                   │                   │
     │ Bearer Token   │                   │                   │
     ├───────────────>│                   │                   │
     │                │                   │                   │
     │           ┌────▼────────┐          │                   │
     │           │  Extract &  │          │                   │
     │           │  Validate   │          │                   │
     │           │    Token    │          │                   │
     │           └────┬────────┘          │                   │
     │                │                   │                   │
     │                │ If Valid          │                   │
     │                ├──────────────────>│                   │
     │                │                   │                   │
     │                │              ┌────▼────────┐          │
     │                │              │   Extract   │          │
     │                │              │ User Claims │          │
     │                │              └────┬────────┘          │
     │                │                   │                   │
     │                │                   │ Business Logic    │
     │                │                   ├──────────────────>│
     │                │                   │                   │
     │                │                   │    Response       │
     │                │                   │<──────────────────┤
     │                │     200 OK        │                   │
     │                │<──────────────────┤                   │
     │   Response     │                   │                   │
     │<───────────────┤                   │                   │
     │                │                   │                   │
```

### 5. Complete Appointment Booking Flow

```
┌─────────┐  ┌──────────┐  ┌────────┐  ┌────────────┐  ┌─────────┐
│ Patient │  │   API    │  │ Doctor │  │ Appointment│  │ Database│
└────┬────┘  └────┬─────┘  └───┬────┘  └─────┬──────┘  └────┬────┘
     │            │            │              │              │
     │  Search    │            │              │              │
     │  Doctors   │            │              │              │
     ├───────────>│            │              │              │
     │            │ Query doctors by          │              │
     │            │ specialization           │              │
     │            ├──────────────────────────────────────────>
     │            │            │              │              │
     │            │       Doctor List         │              │
     │            │<──────────────────────────────────────────┤
     │  Display   │            │              │              │
     │  Doctors   │            │              │              │
     │<───────────┤            │              │              │
     │            │            │              │              │
     │  Book      │            │              │              │
     │  Appointment           │              │              │
     ├───────────>│            │              │              │
     │            │            │   Create     │              │
     │            │            │   Appointment│              │
     │            │            │              ├─────────────>│
     │            │            │              │   Save       │
     │            │            │              │<─────────────┤
     │            │       Confirmation        │              │
     │            │<──────────────────────────┤              │
     │ Confirmation                           │              │
     │<───────────┤            │              │              │
     │            │            │              │              │
```

---

## Business Logic

### 1. Patient Management

**Creating a Patient:**
```csharp
// Validation performed in constructor
var patient = new Patient(
    id: Guid.NewGuid(),
    name: "John Doe",
    email: "john@example.com",
    bloodType: "O+",  // Must be valid: A+, A-, B+, B-, AB+, AB-, O+, O-
    height: 180,      // Must be 50-300 cm
    weight: 75        // Must be 5-500 kg
);
```

**Business Rules:**
- Email must be unique and valid
- Blood type must be one of: A+, A-, B+, B-, AB+, AB-, O+, O-
- Height: 50-300 cm
- Weight: 5-500 kg
- Can have multiple allergies
- Can have multiple medical records
- Can have multiple appointments

**Key Operations:**
- `UpdatePersonalInfo()` - Updates patient details
- `AddAllergy()` - Adds allergy (no duplicates)
- `RemoveAllergy()` - Removes specific allergy

### 2. Doctor Management

**Creating a Doctor:**
```csharp
var doctor = new Doctor(
    id: Guid.NewGuid(),
    name: "Dr. Smith",
    email: "smith@hospital.com",
    licenseNumber: "LIC123456",
    specialization: Specialization.Cardiology,
    yearsOfExperience: 10
);
```

**Business Rules:**
- License number must be unique
- Email must be unique and valid
- Years of experience cannot be negative
- Must be assigned to a hospital (optional)
- Can create and modify medical records
- Can prescribe medications
- Can have multiple appointments

**Key Operations:**
- `AssignToHospital()` - Links doctor to hospital
- `UpdateExperience()` - Updates years of experience

### 3. Appointment Management

**Creating an Appointment:**
```csharp
var appointment = new Appointment(
    id: Guid.NewGuid(),
    appointmentDateTime: DateTime.UtcNow.AddDays(7),
    reasonForVisit: "Annual checkup"
);

appointment.AssignPatient(patient);
appointment.AssignDoctor(doctor);
appointment.SetDuration(60); // minutes
```

**Business Rules:**
- Appointment date must be in the future
- Default duration is 30 minutes
- Statuses: Scheduled → Completed/Cancelled/NoShow
- Cannot reschedule completed or cancelled appointments
- Cannot mark non-scheduled appointments as completed

**State Transitions:**
```
          ┌─────────────┐
          │  Scheduled  │ (Initial state)
          └──────┬──────┘
                 │
      ┌──────────┼──────────┐
      │          │          │
      ▼          ▼          ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│Completed │ │Cancelled │ │  NoShow  │
└──────────┘ └──────────┘ └──────────┘
   (Final)      (Final)      (Final)
```

**Key Operations:**
- `Reschedule()` - Changes appointment time
- `MarkAsCompleted()` - Completes appointment with notes
- `Cancel()` - Cancels appointment with reason
- `MarkAsNoShow()` - Marks patient as no-show

### 4. Medical Record Management

**Creating a Medical Record:**
```csharp
var record = new MedicalRecord(
    id: Guid.NewGuid(),
    diagnosis: "Hypertension",
    symptoms: "High blood pressure, headaches"
);

record.AssignPatient(patient);
record.SetCreatingDoctor(doctor);
record.SetTestsRecommended("Blood test, ECG");
record.SetPhysicalExamination("BP: 140/90");
```

**Business Rules:**
- Must have diagnosis and symptoms
- Created by one doctor
- Can be modified by another doctor (tracks both)
- Contains multiple prescriptions
- Cannot be deleted (audit trail)

**Key Operations:**
- `UpdateMedicalInfo()` - Updates record (tracks modifying doctor)
- `AddPrescription()` - Adds prescription (no duplicates)
- `SetTestsRecommended()` - Sets recommended tests
- `UpdateNotes()` - Adds/updates notes

### 5. Prescription Management

**Creating a Prescription:**
```csharp
var prescription = new Prescription(
    id: Guid.NewGuid(),
    medicationName: "Lisinopril",
    dosage: "10mg",
    frequency: "Once daily",
    durationDays: 30
);

prescription.AssignToMedicalRecord(record);
prescription.SetPrescribingDoctor(doctor);
prescription.SetInstructions("Take with food");
prescription.SetWarnings("May cause dizziness");
```

**Business Rules:**
- Must have medication name, dosage, frequency, duration
- Start date defaults to now
- End date calculated from duration
- Can be active or inactive
- Cannot reactivate expired prescriptions

**Key Operations:**
- `ExtendDuration()` - Adds more days
- `Deactivate()` - Marks as inactive
- `Reactivate()` - Reactivates if not expired

### 6. Allergy Management

**Creating an Allergy:**
```csharp
var allergy = new Allergy(
    id: Guid.NewGuid(),
    name: "Penicillin",
    severity: AllergySeverity.Severe,
    reactionDescription: "Anaphylaxis",
    identifiedDate: DateTime.UtcNow.AddYears(-2)
);

patient.AddAllergy(allergy);
```

**Business Rules:**
- Severity levels: Mild, Moderate, Severe
- Identified date cannot be in the future
- Each patient can have multiple unique allergies

---

## Authentication & Authorization

### JWT Token System

**Access Token** (Short-lived, 60 minutes):
- Contains user claims: Id, Username, Email, Roles
- Used for API authentication
- Stateless - validated via signature

**Refresh Token** (Long-lived, 7 days):
- Stored in database
- Used to get new access token
- Can be revoked
- Tracks IP address

**Token Claims:**
```json
{
  "nameid": "user-guid",
  "name": "johndoe",
  "email": "john@example.com",
  "fullName": "John Doe",
  "isActive": "True",
  "role": "Patient",
  "patientId": "patient-guid",  // if applicable
  "doctorId": "doctor-guid",    // if applicable
  "hospitalId": "hospital-guid" // if applicable
}
```

### Role-Based Access Control

**Roles:**
1. **Patient** - Can view own records, book appointments
2. **Doctor** - Can create records, prescriptions, manage appointments
3. **HospitalAdmin** - Can manage hospital, doctors
4. **SystemAdmin** - Full system access

**Authorization Policies:**
```csharp
// Defined in ServiceExtensions.cs
RequireActiveUser         // User.IsActive = true
RequirePatientRole        // Role: Patient
RequireDoctorRole         // Role: Doctor
RequireHospitalAdminRole  // Role: HospitalAdmin
RequireSystemAdminRole    // Role: SystemAdmin
RequireAdminRole          // Role: HospitalAdmin OR SystemAdmin
RequireMedicalStaff       // Role: Doctor, HospitalAdmin, or SystemAdmin
```

**Usage in Controllers:**
```csharp
[Authorize(Policy = Policies.RequireDoctorRole)]
public async Task<IActionResult> CreateMedicalRecord()
{
    // Only doctors can access
}
```

---

## API Endpoints

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login user | No |
| POST | `/api/auth/refresh-token` | Refresh JWT token | No |
| POST | `/api/auth/forgot-password` | Request password reset | No |
| POST | `/api/auth/reset-password` | Reset password with token | No |
| POST | `/api/auth/change-password` | Change password | Yes |
| GET | `/api/auth/me` | Get current user info | Yes |
| GET | `/api/auth/confirm-email` | Confirm email address | No |

### Request/Response Examples

**Register Request:**
```json
POST /api/auth/register
{
  "userName": "johndoe",
  "email": "john@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "fullName": "John Doe",
  "role": "Patient"
}
```

**Register Response:**
```json
{
  "success": true,
  "message": "User registered successfully.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "base64-encoded-token",
    "expiresAt": "2025-10-28T10:30:00Z",
    "user": {
      "id": "guid",
      "userName": "johndoe",
      "email": "john@example.com",
      "fullName": "John Doe",
      "roles": ["Patient"],
      "patientId": "guid",
      "doctorId": null,
      "hospitalId": null
    }
  },
  "errors": []
}
```

**Login Request:**
```json
POST /api/auth/login
{
  "userNameOrEmail": "john@example.com",
  "password": "Password123!",
  "rememberMe": false
}
```

**Error Response:**
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

---

## Database Schema

### Key Tables

**Users** (ASP.NET Identity)
- Id (PK, Guid)
- UserName
- Email
- PasswordHash
- FullName
- IsActive
- LastLoginDate
- PatientId (FK, nullable)
- DoctorId (FK, nullable)
- HospitalId (FK, nullable)

**Patients**
- Id (PK, Guid)
- Name
- Email (Unique)
- BloodType
- Height
- Weight
- CreatedDate
- UpdatedDate

**Doctors**
- Id (PK, Guid)
- Name
- Email
- LicenseNumber (Unique)
- Specialization
- YearsOfExperience
- HospitalId (FK, nullable)
- CreatedDate
- UpdatedDate

**Hospitals**
- Id (PK, Guid)
- Name (Unique)
- RegistrationNumber (Unique)
- Address
- City
- PhoneNumber

**Appointments**
- Id (PK, Guid)
- PatientId (FK)
- DoctorId (FK)
- AppointmentDateTime
- DurationMinutes
- Status
- Notes
- ReasonForVisit

**MedicalRecords**
- Id (PK, Guid)
- PatientId (FK)
- CreatedByDoctorId (FK)
- ModifiedByDoctorId (FK, nullable)
- Diagnosis
- Symptoms
- Treatment
- TestsRecommended
- PhysicalExamination
- Notes

**Prescriptions**
- Id (PK, Guid)
- MedicalRecordId (FK)
- PrescribedByDoctorId (FK)
- MedicationName
- GenericName
- Dosage
- Frequency
- DurationDays
- StartDate
- EndDate
- Quantity
- Instructions
- Warnings
- IsActive

**Allergies**
- Id (PK, Guid)
- PatientId (FK)
- Name
- Severity
- ReactionDescription
- IdentifiedDate

**RefreshTokens**
- Id (PK, Guid)
- UserId (FK)
- RefreshTokenValue (Unique)
- ExpiryDate
- IpAddress
- IsActive
- CreatedAt

### Relationship Cardinalities

```
User (1) ─────? (0..1) Patient
User (1) ─────? (0..1) Doctor
User (1) ─────? (0..1) Hospital

Hospital (1) ────── (N) Doctor
Patient (1) ────── (N) Allergy
Patient (1) ────── (N) Appointment ────── (1) Doctor
Patient (1) ────── (N) MedicalRecord ────── (1) Doctor (Creator)
MedicalRecord (1) ────── (N) Prescription ────── (1) Doctor (Prescriber)
```

---

## Testing Strategy

### Unit Tests (HealthLink.Tests)

**Entity Tests:**
- Validate business rules in constructors
- Test state transitions (e.g., Appointment statuses)
- Verify relationships and navigation properties
- Test domain method behaviors

**Service Tests:**
- Mock dependencies (UserManager, SignInManager, IJwtService)
- Test authentication flows
- Verify error handling
- Test token generation and validation

**Example Test:**
```csharp
[Fact]
public async Task RegisterAsync_WithValidData_ShouldSucceed()
{
    // Arrange
    var request = new RegisterRequest { ... };
    _userManagerMock.Setup(...).ReturnsAsync(IdentityResult.Success);
    
    // Act
    var result = await _authService.RegisterAsync(request);
    
    // Assert
    result.Success.Should().BeTrue();
    result.Data.Should().NotBeNull();
}
```

---

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=healthlink_db;Username=postgres;Password=***"
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

### Identity Configuration

```csharp
// Password Requirements (ServiceExtensions.cs)
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 6;
options.Password.RequiredUniqueChars = 1;

// Lockout Settings
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;

// User Settings
options.User.RequireUniqueEmail = true;
options.SignIn.RequireConfirmedEmail = false;
```

---

## Detailed Business Workflows

### Workflow 1: Complete Patient Journey

```
┌──────────────────────────────────────────────────────────────┐
│                    Patient Registration                       │
│  1. User registers with Role = "Patient"                     │
│  2. System creates User entity                               │
│  3. System can later link to Patient entity (PatientId)      │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Search for Doctor                          │
│  1. Patient browses doctors by specialization                │
│  2. Views doctor experience, hospital affiliation            │
│  3. Checks available appointment slots                       │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Book Appointment                           │
│  1. Select doctor and time slot                              │
│  2. Provide reason for visit                                 │
│  3. System creates Appointment (Status: Scheduled)           │
│  4. Links appointment to Patient and Doctor                  │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                  Appointment Consultation                     │
│  1. Doctor marks appointment as Completed                    │
│  2. Doctor creates Medical Record                            │
│     - Enters diagnosis                                       │
│     - Documents symptoms                                     │
│     - Records physical examination                           │
│     - Recommends tests                                       │
│  3. Doctor adds Prescriptions to Medical Record              │
│     - Medication names, dosages                              │
│     - Frequency and duration                                 │
│     - Instructions and warnings                              │
│  4. Doctor documents any Allergies discovered                │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Follow-up Care                             │
│  1. Patient can view medical records                         │
│  2. Patient can view active prescriptions                    │
│  3. Patient can book follow-up appointment                   │
│  4. Another doctor can modify the medical record             │
│     (tracked via ModifiedByDoctor)                           │
└──────────────────────────────────────────────────────────────┘
```

### Workflow 2: Doctor's Daily Operations

```
┌──────────────────────────────────────────────────────────────┐
│                    Morning - View Schedule                    │
│  1. Doctor logs in                                           │
│  2. Views today's appointments                               │
│  3. Reviews patient information for each appointment         │
│     - Previous medical records                               │
│     - Known allergies                                        │
│     - Active prescriptions                                   │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Patient Consultation                       │
│  1. Marks appointment as Started (if implemented)            │
│  2. Conducts examination                                     │
│  3. Creates new Medical Record                               │
│     - Links to patient                                       │
│     - Sets self as CreatedByDoctor                           │
│  4. Adds diagnosis based on findings                         │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Prescribe Medications                      │
│  1. Creates Prescription entities                            │
│  2. Links to Medical Record                                  │
│  3. Sets self as PrescribedByDoctor                          │
│  4. Specifies:                                               │
│     - Medication name and generic name                       │
│     - Dosage and frequency                                   │
│     - Duration and quantity                                  │
│     - Special instructions                                   │
│     - Warnings and contraindications                         │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Complete Appointment                       │
│  1. Marks appointment as Completed                           │
│  2. Adds consultation notes                                  │
│  3. Records any recommendations                              │
│  4. Schedules follow-up if needed                            │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    End of Day                                 │
│  1. Reviews all completed appointments                       │
│  2. Checks for no-shows                                      │
│  3. Prepares for next day                                    │
└──────────────────────────────────────────────────────────────┘
```

### Workflow 3: Hospital Administration

```
┌──────────────────────────────────────────────────────────────┐
│                    Hospital Setup                             │
│  1. System admin creates Hospital entity                     │
│  2. Sets hospital details:                                   │
│     - Name, registration number                              │
│     - Address, city, phone                                   │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Hire Doctors                               │
│  1. Hospital admin creates Doctor entity                     │
│  2. Assigns doctor to hospital via AssignToHospital()        │
│  3. Doctor entity links to hospital                          │
│  4. Creates User account for doctor                          │
│     - Role: Doctor                                           │
│     - Links User.DoctorId to Doctor.Id                       │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    Manage Operations                          │
│  1. Monitor appointment statistics                           │
│  2. View doctor schedules                                    │
│  3. Track patient records                                    │
│  4. Generate reports (if implemented)                        │
└──────────────────────────────────────────────────────────────┘
```

---

## Security Considerations

### 1. Authentication Security

**Password Storage:**
- Passwords hashed using ASP.NET Identity (PBKDF2)
- Never stored in plain text
- Password complexity requirements enforced

**JWT Token Security:**
- Tokens signed with HMAC-SHA256
- Secret key stored in configuration (should be in environment variables in production)
- Short token expiration (60 minutes)
- Refresh token rotation

**Account Lockout:**
- 5 failed login attempts → 15 minute lockout
- Prevents brute force attacks

### 2. Authorization

**Role-Based Access:**
- Each endpoint protected by role requirements
- Claims-based authorization for fine-grained control
- Active user check via custom policy

**Example Protected Endpoint:**
```csharp
[Authorize(Policy = Policies.RequireDoctorRole)]
[Authorize(Policy = Policies.RequireActiveUser)]
public async Task<IActionResult> CreateMedicalRecord()
{
    // Only active doctors can access
}
```

### 3. Data Protection

**Sensitive Data:**
- Medical records access restricted
- Patient data only accessible by:
  - The patient themselves
  - Doctors with appointments
  - Doctors who created records

**Audit Trail:**
- All entities have CreatedDate, UpdatedDate
- Medical records track creator and modifier doctors
- Login activity tracked via LastLoginDate

### 4. Input Validation

**Multiple Layers:**
1. **Model Validation** - Data annotations
2. **Business Logic Validation** - Entity constructors
3. **Database Constraints** - Unique indexes, FK constraints

**Example:**
```csharp
// Model validation
[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Invalid email format")]
public string Email { get; set; }

// Business validation
if (!IsValidEmail(email))
    throw new ArgumentException("Email must be valid");

// Database constraint
entity.HasIndex(e => e.Email).IsUnique();
```

---

## Common Operations & Code Examples

### 1. Creating a Complete Patient Record

```csharp
// Step 1: Create Patient entity
var patient = new Patient(
    id: Guid.NewGuid(),
    name: "John Doe",
    email: "john@example.com",
    bloodType: "O+",
    height: 180,
    weight: 75
);

// Step 2: Add allergies
var allergy = new Allergy(
    id: Guid.NewGuid(),
    name: "Penicillin",
    severity: AllergySeverity.Severe,
    reactionDescription: "Anaphylaxis"
);
patient.AddAllergy(allergy);

// Step 3: Save to database
await _context.Patients.AddAsync(patient);
await _context.SaveChangesAsync();
```

### 2. Doctor Creating Medical Record

```csharp
// Step 1: Find patient and doctor
var patient = await _context.Patients.FindAsync(patientId);
var doctor = await _context.Doctors.FindAsync(doctorId);

// Step 2: Create medical record
var record = new MedicalRecord(
    id: Guid.NewGuid(),
    diagnosis: "Hypertension Stage 1",
    symptoms: "High BP, occasional headaches"
);

record.AssignPatient(patient);
record.SetCreatingDoctor(doctor);
record.SetPhysicalExamination("BP: 140/90, HR: 78");
record.SetTestsRecommended("Lipid panel, ECG");

// Step 3: Add prescriptions
var prescription = new Prescription(
    id: Guid.NewGuid(),
    medicationName: "Lisinopril",
    dosage: "10mg",
    frequency: "Once daily",
    durationDays: 30
);

prescription.AssignToMedicalRecord(record);
prescription.SetPrescribingDoctor(doctor);
prescription.SetInstructions("Take in the morning with water");
prescription.SetWarnings("May cause dizziness initially");

record.AddPrescription(prescription);

// Step 4: Save to database
await _context.MedicalRecords.AddAsync(record);
await _context.SaveChangesAsync();
```

### 3. Booking an Appointment

```csharp
// Step 1: Create appointment
var appointment = new Appointment(
    id: Guid.NewGuid(),
    appointmentDateTime: DateTime.UtcNow.AddDays(7),
    reasonForVisit: "Follow-up consultation"
);

// Step 2: Assign patient and doctor
var patient = await _context.Patients.FindAsync(patientId);
var doctor = await _context.Doctors.FindAsync(doctorId);

appointment.AssignPatient(patient);
appointment.AssignDoctor(doctor);
appointment.SetDuration(45); // 45 minutes

// Step 3: Save
await _context.Appointments.AddAsync(appointment);
await _context.SaveChangesAsync();
```

### 4. Completing an Appointment

```csharp
// Step 1: Find appointment
var appointment = await _context.Appointments
    .Include(a => a.Patient)
    .Include(a => a.Doctor)
    .FirstOrDefaultAsync(a => a.Id == appointmentId);

if (appointment == null)
    throw new NotFoundException("Appointment not found");

// Step 2: Mark as completed
appointment.MarkAsCompleted(
    "Patient responded well to treatment. " +
    "Blood pressure improved. Continue current medication."
);

// Step 3: Update database
_context.Appointments.Update(appointment);
await _context.SaveChangesAsync();
```

---

## Error Handling

### Standardized API Response

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
}
```

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "errors": []
}
```

### Error Response
```json
{
  "success": false,
  "message": "Operation failed",
  "data": null,
  "errors": [
    "Email already exists",
    "Password too weak"
  ]
}
```

### HTTP Status Codes

| Status | When Used |
|--------|-----------|
| 200 OK | Successful GET, PUT, POST |
| 400 Bad Request | Validation errors, invalid input |
| 401 Unauthorized | Missing or invalid authentication |
| 403 Forbidden | Authenticated but not authorized |
| 404 Not Found | Resource doesn't exist |
| 500 Internal Server Error | Unhandled exceptions |

---

## Entity State Management

### Entity Lifecycle

```
┌──────────────┐
│   Detached   │ ← Entity created but not tracked
└──────┬───────┘
       │ Add to DbContext
       ▼
┌──────────────┐
│    Added     │ ← New entity, will be inserted
└──────┬───────┘
       │ SaveChanges()
       ▼
┌──────────────┐
│  Unchanged   │ ← Loaded from DB, no modifications
└──────┬───────┘
       │ Modify property
       ▼
┌──────────────┐
│   Modified   │ ← Changes tracked, will be updated
└──────┬───────┘
       │ SaveChanges()
       ▼
┌──────────────┐
│  Unchanged   │
└──────────────┘
```

### Lazy Loading

The application uses lazy loading proxies:
```csharp
// In Program.cs
options.UseNpgsql(connectionString)
       .UseLazyLoadingProxies();
```

**Benefits:**
- Navigation properties loaded on access
- Reduces initial query size

**Considerations:**
- Can cause N+1 query problems
- Use `.Include()` for eager loading when needed

**Example:**
```csharp
// Lazy loading (multiple queries)
var patient = await _context.Patients.FindAsync(id);
var allergies = patient.Allergies; // Separate query

// Eager loading (single query)
var patient = await _context.Patients
    .Include(p => p.Allergies)
    .Include(p => p.MedicalRecords)
    .FirstOrDefaultAsync(p => p.Id == id);
```

---

## Development Workflow

### 1. Adding a New Entity

**Step 1: Create Entity Class**
```csharp
// In HealthLink.Core/Entities/
public class Medication : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public Medication(Guid id, string name) : base(id)
    {
        // Validation
        Name = name;
    }
}
```

**Step 2: Add DbSet to Context**
```csharp
// In HealthLinkDbContext
public DbSet<Medication> Medications { get; set; }
```

**Step 3: Configure Entity**
```csharp
// In OnModelCreating
private void ConfigureMedication(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Medication>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.HasIndex(e => e.Name).IsUnique();
    });
}
```

**Step 4: Create Migration**
```bash
dotnet ef migrations add AddMedication --project HealthLink.Data --startup-project HealthLink.API
```

**Step 5: Update Database**
```bash
dotnet ef database update --project HealthLink.Data --startup-project HealthLink.API
```

### 2. Adding a New API Endpoint

**Step 1: Create Request/Response DTOs**
```csharp
// In HealthLink.Core/Models/
public class CreateMedicationRequest
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
}
```

**Step 2: Create Service Method**
```csharp
// In HealthLink.Business/Services/
public async Task<ApiResponse<Medication>> CreateMedicationAsync(
    CreateMedicationRequest request)
{
    var medication = new Medication(Guid.NewGuid(), request.Name);
    await _context.Medications.AddAsync(medication);
    await _context.SaveChangesAsync();
    
    return ApiResponse<Medication>.SuccessResponse(
        medication, 
        "Medication created"
    );
}
```

**Step 3: Create Controller**
```csharp
// In HealthLink.API/Controllers/
[ApiController]
[Route("api/[controller]")]
public class MedicationsController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Policies.RequireDoctorRole)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMedicationRequest request)
    {
        var result = await _service.CreateMedicationAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
```

**Step 4: Write Tests**
```csharp
// In HealthLink.Tests/
[Fact]
public async Task CreateMedication_WithValidData_ShouldSucceed()
{
    // Arrange
    var request = new CreateMedicationRequest { Name = "Aspirin" };
    
    // Act
    var result = await _service.CreateMedicationAsync(request);
    
    // Assert
    result.Success.Should().BeTrue();
}
```

---

## Future Enhancements

### Potential Features

1. **Doctor Schedule Management**
   - Time slot availability
   - Working hours configuration
   - Appointment conflicts prevention

2. **Payment Processing**
   - Consultation fees
   - Payment history
   - Insurance claims

3. **Notifications**
   - Email/SMS for appointments
   - Prescription reminders
   - Test result notifications

4. **Medical Tests**
   - Test ordering and tracking
   - Lab results integration
   - Result interpretation

5. **Telemedicine**
   - Video consultations
   - Chat messaging
   - Digital prescriptions

6. **Analytics & Reporting**
   - Patient health trends
   - Doctor performance metrics
   - Hospital statistics

7. **Document Management**
   - Medical report uploads
   - Test result PDFs
   - Prescription images

8. **Advanced Search**
   - Search doctors by availability
   - Filter by insurance accepted
   - Location-based search

---

## Deployment Considerations

### Environment Configuration

**Development:**
- Local PostgreSQL database
- Debug logging enabled
- Swagger UI enabled
- CORS allows all origins

**Production:**
- Cloud database (Azure PostgreSQL, AWS RDS)
- Structured logging (Serilog to cloud)
- Swagger disabled
- CORS restricted to specific domains
- HTTPS enforced
- Secrets in environment variables/Key Vault

### Connection String Security

**Don't commit:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod;Database=healthlink;User=admin;Password=secret123"
  }
}
```

**Use environment variables:**
```csharp
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];
```

### JWT Secret Management

**Development:** appsettings.Development.json (gitignored)
**Production:** Environment variables or Azure Key Vault

```bash
# Environment variable
export JWT_SECRET_KEY="YourProductionSecretKey..."

# In code
jwtSettings.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
    ?? jwtSettings.SecretKey;
```

---

## Conclusion

HealthLink is a well-structured healthcare management system built on Clean Architecture principles. The application separates concerns effectively across layers, uses strong typing and domain-driven design, and implements secure authentication and authorization.

Key strengths:
- **Domain-Rich Entities**: Business logic embedded in entities
- **Comprehensive Testing**: Unit tests for entities and services
- **Security-First**: JWT authentication, role-based authorization
- **Maintainability**: Clear layer separation, dependency injection
- **Scalability**: Stateless API design, token-based auth

The system provides a solid foundation for healthcare management and can be extended with additional features like telemedicine, analytics, and advanced scheduling.