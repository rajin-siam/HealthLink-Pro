using HealthLink.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthLink.Data.Context
{
    /// <summary>
    /// Database context for the HealthLink application.
    /// Now properly inherits from IdentityDbContext to support ASP.NET Core Identity.
    /// </summary>
    public class HealthLinkDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        // DbSets for domain entities
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public HealthLinkDbContext(DbContextOptions<HealthLinkDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: Call base first to configure Identity tables
            base.OnModelCreating(modelBuilder);

            // Configure Identity table names for better organization
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            // Configure domain entities
            ConfigureUser(modelBuilder);
            ConfigurePatient(modelBuilder);
            ConfigureDoctor(modelBuilder);
            ConfigureHospital(modelBuilder);
            ConfigureAppointment(modelBuilder);
            ConfigureMedicalRecord(modelBuilder);
            ConfigurePrescription(modelBuilder);
            ConfigureAllergy(modelBuilder);
            ConfigureRefreshToken(modelBuilder);
        }

        /// <summary>
        /// Configures the User entity relationships with domain entities.
        /// This links authentication users to their domain roles (Patient, Doctor, Hospital).
        /// </summary>
        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // User properties
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedDate)
                    .IsRequired();

                entity.Property(e => e.UpdatedDate)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // One User can be linked to One Patient (optional)
                entity.HasOne(e => e.Patient)
                    .WithMany()
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // One User can be linked to One Doctor (optional)
                entity.HasOne(e => e.Doctor)
                    .WithMany()
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // One User can be linked to One Hospital (optional)
                entity.HasOne(e => e.Hospital)
                    .WithMany()
                    .HasForeignKey(e => e.HospitalId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Indexes
                entity.HasIndex(e => e.PatientId)
                    .IsUnique()
                    .HasFilter("[PatientId] IS NOT NULL")
                    .HasDatabaseName("IX_Users_PatientId");

                entity.HasIndex(e => e.DoctorId)
                    .IsUnique()
                    .HasFilter("[DoctorId] IS NOT NULL")
                    .HasDatabaseName("IX_Users_DoctorId");

                entity.HasIndex(e => e.HospitalId)
                    .IsUnique()
                    .HasFilter("[HospitalId] IS NOT NULL")
                    .HasDatabaseName("IX_Users_HospitalId");

                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_Users_IsActive");
            });
        }

        private void ConfigurePatient(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.BloodType)
                    .HasMaxLength(10);

                entity.Property(e => e.Height)
                    .HasPrecision(5, 2);

                entity.Property(e => e.Weight)
                    .HasPrecision(5, 2);

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Patients_Email");

                entity.HasIndex(e => e.CreatedDate)
                    .HasDatabaseName("IX_Patients_CreatedDate");

                entity.HasMany(e => e.Allergies)
                    .WithOne(a => a.Patient)
                    .HasForeignKey("PatientId")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.MedicalRecords)
                    .WithOne(m => m.Patient)
                    .HasForeignKey("PatientId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Appointments)
                    .WithOne(a => a.Patient)
                    .HasForeignKey("PatientId")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureDoctor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.LicenseNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Specialization)
                    .HasConversion<string>()
                    .IsRequired();

                entity.HasIndex(e => e.LicenseNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Doctors_LicenseNumber");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_Doctors_Email");

                entity.HasIndex(e => e.Specialization)
                    .HasDatabaseName("IX_Doctors_Specialization");

                entity.HasOne(e => e.Hospital)
                    .WithMany(h => h.Doctors)
                    .HasForeignKey("HospitalId")
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                entity.HasMany(e => e.Appointments)
                    .WithOne(a => a.Doctor)
                    .HasForeignKey("DoctorId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.CreatedMedicalRecords)
                    .WithOne(m => m.CreatedByDoctor)
                    .HasForeignKey("CreatedByDoctorId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.ModifiedMedicalRecords)
                    .WithOne(m => m.ModifiedByDoctor)
                    .HasForeignKey("ModifiedByDoctorId")
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                entity.HasMany(e => e.Prescriptions)
                    .WithOne(p => p.PrescribedByDoctor)
                    .HasForeignKey("PrescribedByDoctorId")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureHospital(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.RegistrationNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Hospitals_Name");

                entity.HasIndex(e => e.RegistrationNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Hospitals_RegistrationNumber");
            });
        }

        private void ConfigureAppointment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AppointmentDateTime)
                    .IsRequired();

                entity.Property(e => e.DurationMinutes)
                    .HasDefaultValue(30);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.ReasonForVisit)
                    .HasMaxLength(500);

                entity.HasIndex(e => e.AppointmentDateTime)
                    .HasDatabaseName("IX_Appointments_AppointmentDateTime");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Appointments_Status");
            });
        }

        private void ConfigureMedicalRecord(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Diagnosis)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Symptoms)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.TestsRecommended)
                    .HasMaxLength(2000);

                entity.Property(e => e.PhysicalExamination)
                    .HasMaxLength(2000);

                entity.Property(e => e.Treatment)
                    .HasMaxLength(2000);

                entity.Property(e => e.Notes)
                    .HasMaxLength(3000);

                entity.HasIndex(e => e.CreatedDate)
                    .HasDatabaseName("IX_MedicalRecords_CreatedDate");

                entity.HasMany(e => e.Prescriptions)
                    .WithOne(p => p.MedicalRecord)
                    .HasForeignKey("MedicalRecordId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurePrescription(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MedicationName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.GenericName)
                    .HasMaxLength(255);

                entity.Property(e => e.Dosage)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Frequency)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Instructions)
                    .HasMaxLength(1000);

                entity.Property(e => e.Warnings)
                    .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_Prescriptions_IsActive");

                entity.HasIndex(e => e.EndDate)
                    .HasDatabaseName("IX_Prescriptions_EndDate");
            });
        }

        private void ConfigureAllergy(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Allergy>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Severity)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.ReactionDescription)
                    .HasMaxLength(1000);

                entity.Property(e => e.IdentifiedDate)
                    .IsRequired();

                entity.HasIndex(e => e.Severity)
                    .HasDatabaseName("IX_Allergies_Severity");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Allergies_Name");
            });
        }

        private void ConfigureRefreshToken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RefreshTokenValue)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ExpiryDate)
                    .IsRequired();

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Add index for token lookup
                entity.HasIndex(e => e.RefreshTokenValue)
                    .IsUnique()
                    .HasDatabaseName("IX_RefreshTokens_Token");

                // Add index for expiry queries
                entity.HasIndex(e => new { e.IsActive, e.ExpiryDate })
                    .HasDatabaseName("IX_RefreshTokens_Active_Expiry");

                // Configure relationship with User
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}