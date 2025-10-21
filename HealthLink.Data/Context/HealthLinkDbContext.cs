using Microsoft.EntityFrameworkCore;
using HealthLink.Core.Entities;

namespace HealthLink.Data.Context
{
    /// <summary>
    /// Database context for the HealthLink application.
    /// Configures all entity relationships and database constraints.
    /// </summary>
    public class HealthLinkDbContext : DbContext
    {
        // DbSets represent tables in the database
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Allergy> Allergies { get; set; }

        public HealthLinkDbContext(DbContextOptions<HealthLinkDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure each entity and their relationships
            ConfigurePatient(modelBuilder);
            ConfigureDoctor(modelBuilder);
            ConfigureHospital(modelBuilder);
            ConfigureAppointment(modelBuilder);
            ConfigureMedicalRecord(modelBuilder);
            ConfigurePrescription(modelBuilder);
            ConfigureAllergy(modelBuilder);
        }

        /// <summary>
        /// Configures the Patient entity and its relationships.
        /// </summary>
        private void ConfigurePatient(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.BloodType)
                    .HasMaxLength(10);

                entity.Property(e => e.Height)
                    .HasPrecision(5, 2); // 5 total digits, 2 after decimal (e.g., 180.50)

                entity.Property(e => e.Weight)
                    .HasPrecision(5, 2);

                // Indexes for Performance
                entity.HasIndex(e => e.Email)
                    .IsUnique() // Ensures no duplicate emails
                    .HasDatabaseName("IX_Patients_Email");

                entity.HasIndex(e => e.CreatedDate)
                    .HasDatabaseName("IX_Patients_CreatedDate");

                // Relationships
                // One Patient has many Allergies
                entity.HasMany(e => e.Allergies)
                    .WithOne(a => a.Patient)
                    .HasForeignKey("PatientId") // Shadow property - EF creates this automatically
                    .OnDelete(DeleteBehavior.Cascade); // Delete allergies when patient is deleted

                // One Patient has many MedicalRecords
                entity.HasMany(e => e.MedicalRecords)
                    .WithOne(m => m.Patient)
                    .HasForeignKey("PatientId")
                    .OnDelete(DeleteBehavior.Restrict); // Don't delete records when patient is deleted

                // One Patient has many Appointments
                entity.HasMany(e => e.Appointments)
                    .WithOne(a => a.Patient)
                    .HasForeignKey("PatientId")
                    .OnDelete(DeleteBehavior.Restrict); // Don't delete appointments when patient is deleted
            });
        }

        /// <summary>
        /// Configures the Doctor entity and its relationships.
        /// </summary>
        private void ConfigureDoctor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.LicenseNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                // Enum stored as string for readability in database
                entity.Property(e => e.Specialization)
                    .HasConversion<string>()
                    .IsRequired();

                // Indexes for Performance
                entity.HasIndex(e => e.LicenseNumber)
                    .IsUnique() // Each doctor has unique license
                    .HasDatabaseName("IX_Doctors_LicenseNumber");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_Doctors_Email");

                entity.HasIndex(e => e.Specialization)
                    .HasDatabaseName("IX_Doctors_Specialization");

                // Relationships
                // Many Doctors belong to One Hospital
                entity.HasOne(e => e.Hospital)
                    .WithMany(h => h.Doctors)
                    .HasForeignKey("HospitalId") // Shadow property
                    .OnDelete(DeleteBehavior.SetNull) // Set to null if hospital is deleted
                    .IsRequired(false); // Hospital is optional (doctor may not have hospital yet)

                // One Doctor has many Appointments
                entity.HasMany(e => e.Appointments)
                    .WithOne(a => a.Doctor)
                    .HasForeignKey("DoctorId")
                    .OnDelete(DeleteBehavior.Restrict);

                // One Doctor creates many MedicalRecords
                entity.HasMany(e => e.CreatedMedicalRecords)
                    .WithOne(m => m.CreatedByDoctor)
                    .HasForeignKey("CreatedByDoctorId")
                    .OnDelete(DeleteBehavior.Restrict);

                // One Doctor can modify many MedicalRecords
                entity.HasMany(e => e.ModifiedMedicalRecords)
                    .WithOne(m => m.ModifiedByDoctor)
                    .HasForeignKey("ModifiedByDoctorId")
                    .OnDelete(DeleteBehavior.SetNull) // Can be null
                    .IsRequired(false);

                // One Doctor prescribes many Prescriptions
                entity.HasMany(e => e.Prescriptions)
                    .WithOne(p => p.PrescribedByDoctor)
                    .HasForeignKey("PrescribedByDoctorId")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        /// <summary>
        /// Configures the Hospital entity and its relationships.
        /// </summary>
        private void ConfigureHospital(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hospital>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
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

                // Indexes for Performance
                entity.HasIndex(e => e.Name)
                    .IsUnique() // Each hospital has unique name
                    .HasDatabaseName("IX_Hospitals_Name");

                entity.HasIndex(e => e.RegistrationNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Hospitals_RegistrationNumber");

                // Relationships are configured in Doctor configuration (One-to-Many)
                // No need to configure here as it's already done from the "Many" side
            });
        }

        /// <summary>
        /// Configures the Appointment entity and its relationships.
        /// </summary>
        private void ConfigureAppointment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
                entity.Property(e => e.AppointmentDateTime)
                    .IsRequired();

                entity.Property(e => e.DurationMinutes)
                    .HasDefaultValue(30);

                // Enum stored as string
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.ReasonForVisit)
                    .HasMaxLength(500);

                // Indexes for Performance
                entity.HasIndex(e => e.AppointmentDateTime)
                    .HasDatabaseName("IX_Appointments_AppointmentDateTime");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Appointments_Status");

                // Composite index for common queries (finding appointments for a patient)
                entity.HasIndex(e => new { e.AppointmentDateTime })
                    .HasDatabaseName("IX_Appointments_PatientDoctor");

                // Relationships are configured in Patient and Doctor configurations
                // The FK shadow properties PatientId and DoctorId are created automatically
            });
        }

        /// <summary>
        /// Configures the MedicalRecord entity and its relationships.
        /// </summary>
        private void ConfigureMedicalRecord(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
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

                // Indexes for Performance
                entity.HasIndex(e => e.CreatedDate)
                    .HasDatabaseName("IX_MedicalRecords_CreatedDate");

                // Composite index for finding records by patient
                entity.HasIndex(e => new { e.CreatedDate })
                    .HasDatabaseName("IX_MedicalRecords_PatientCreatedDate");

                // Relationships
                // One MedicalRecord has many Prescriptions
                entity.HasMany(e => e.Prescriptions)
                    .WithOne(p => p.MedicalRecord)
                    .HasForeignKey("MedicalRecordId")
                    .OnDelete(DeleteBehavior.Cascade); // Delete prescriptions when record is deleted

                // Relationships with Patient and Doctors are configured in their respective configurations
            });
        }

        /// <summary>
        /// Configures the Prescription entity and its relationships.
        /// </summary>
        private void ConfigurePrescription(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prescription>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
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

                // Indexes for Performance
                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_Prescriptions_IsActive");

                entity.HasIndex(e => e.EndDate)
                    .HasDatabaseName("IX_Prescriptions_EndDate");

                // Composite index for finding active prescriptions
                entity.HasIndex(e => new { e.IsActive, e.EndDate })
                    .HasDatabaseName("IX_Prescriptions_ActiveEndDate");

                // Relationships are configured in MedicalRecord and Doctor configurations
            });
        }

        /// <summary>
        /// Configures the Allergy entity and its relationships.
        /// </summary>
        private void ConfigureAllergy(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Allergy>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties Configuration
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                // Enum stored as string
                entity.Property(e => e.Severity)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.ReactionDescription)
                    .HasMaxLength(1000);

                entity.Property(e => e.IdentifiedDate)
                    .IsRequired();

                // Indexes for Performance
                entity.HasIndex(e => e.Severity)
                    .HasDatabaseName("IX_Allergies_Severity");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Allergies_Name");

                // Relationship with Patient is configured in Patient configuration
            });
        }
    }
}