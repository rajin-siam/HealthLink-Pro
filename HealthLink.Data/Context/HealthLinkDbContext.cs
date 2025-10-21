using Microsoft.EntityFrameworkCore;
using HealthLink.Core.Entities;

namespace HealthLink.Data.Context
{
    public class HealthLinkDbContext : DbContext
    {
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

            // Configure Patient entity
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.BloodType).HasMaxLength(10);
                entity.Property(e => e.Height).HasPrecision(5, 2);
                entity.Property(e => e.Weight).HasPrecision(5, 2);
                entity.HasMany(e => e.Allergies)
                    .WithOne()
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Add indexes for PostgreSQL
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure Doctor entity
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Specialization).HasConversion<string>();

                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.Specialization);
            });

            // Configure Hospital entity
            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(100);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            });

            // Configure Appointment entity
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.AppointmentDateTime);
                entity.HasIndex(e => e.Status);
            });

            // Configure MedicalRecord entity
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Diagnosis).IsRequired();
                entity.Property(e => e.Symptoms).IsRequired();

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.CreatedByDoctorId);
                entity.HasMany(e => e.Prescriptions)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Prescription entity
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MedicationName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Dosage).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Frequency).IsRequired().HasMaxLength(100);

                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.EndDate);
            });

            // Configure Allergy entity
            modelBuilder.Entity<Allergy>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Severity).HasConversion<string>();
                entity.Property(e => e.PatientId).IsRequired();

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Severity);
            });
        }
    }
}