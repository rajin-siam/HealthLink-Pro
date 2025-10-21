using Microsoft.AspNetCore.Identity;

namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents a user in the system with authentication and authorization.
    /// Extends IdentityUser to include custom properties.
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        // Custom Properties
        public string FullName { get; private set; } = string.Empty;
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginDate { get; private set; }

        // Optional reference to domain entities
        public Guid? PatientId { get; private set; }
        public Guid? DoctorId { get; private set; }
        public Guid? HospitalId { get; private set; }

        // Navigation properties
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
        public virtual Hospital? Hospital { get; set; }

        // Constructor
        public User(string userName, string email, string fullName)
        {
            Id = Guid.NewGuid();
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            IsActive = true;
            EmailConfirmed = false;
        }

        // Parameterless constructor for EF Core
        protected User()
        {
            Id = Guid.NewGuid();
            UserName = string.Empty;
            Email = string.Empty;
            FullName = string.Empty;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            IsActive = true;
            EmailConfirmed = false;
        }

        // Business Methods (remain the same)
        public void UpdateProfile(string fullName, string email)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            FullName = fullName;
            Email = email;
            UpdatedDate = DateTime.UtcNow;
        }

        public void LinkToPatient(Guid patientId)
        {
            if (DoctorId.HasValue || HospitalId.HasValue)
                throw new InvalidOperationException("User is already linked to another entity.");

            PatientId = patientId;
            UpdatedDate = DateTime.UtcNow;
        }

        public void LinkToDoctor(Guid doctorId)
        {
            if (PatientId.HasValue || HospitalId.HasValue)
                throw new InvalidOperationException("User is already linked to another entity.");

            DoctorId = doctorId;
            UpdatedDate = DateTime.UtcNow;
        }

        public void LinkToHospital(Guid hospitalId)
        {
            if (PatientId.HasValue || DoctorId.HasValue)
                throw new InvalidOperationException("User is already linked to another entity.");

            HospitalId = hospitalId;
            UpdatedDate = DateTime.UtcNow;
        }

        public void RecordLogin()
        {
            LastLoginDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}