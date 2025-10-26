using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents a doctor in the healthcare system.
    /// Doctors can create medical records, prescribe medications, and have appointments.
    /// </summary>
    public class Doctor : BaseEntity
    {
        // =========================
        // Basic Information
        // =========================
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string LicenseNumber { get; private set; }
        public Specialization Specialization { get; private set; }
        public int YearsOfExperience { get; private set; }

        // =========================
        // Contact & Location
        // =========================
        public string PhoneNumber { get; private set; }           // Patients can call
        public string OfficeAddress { get; private set; }         // Where to find them

        // =========================
        // Professional Profile
        // =========================
        public string Education { get; private set; }             // e.g., "MD from Harvard"
        public string Biography { get; private set; }             // Profile page summary

        // =========================
        // Business Information
        // =========================
        public decimal ConsultationFee { get; private set; }      // Pricing info
        public bool IsAvailable { get; private set; } = true;     // Availability flag
        public TimeSpan ConsultationDuration { get; private set; } = TimeSpan.FromMinutes(30);

        // =========================
        // Relationships
        // =========================
        public virtual Hospital Hospital { get; private set; }
        public virtual ICollection<Appointment> Appointments { get; private set; }
        public virtual ICollection<MedicalRecord> CreatedMedicalRecords { get; private set; }
        public virtual ICollection<MedicalRecord> ModifiedMedicalRecords { get; private set; }
        public virtual ICollection<Prescription> Prescriptions { get; private set; }

        // =========================
        // Constructors
        // =========================
        public Doctor(Guid id, string name, string email, string licenseNumber,
                      Specialization specialization, int yearsOfExperience,
                      string phoneNumber, string officeAddress,
                      string education, string biography,
                      decimal consultationFee, TimeSpan? consultationDuration = null)
            : base(id)
        {
            ValidateDoctorData(name, email, licenseNumber, yearsOfExperience);

            Name = name;
            Email = email;
            LicenseNumber = licenseNumber;
            Specialization = specialization;
            YearsOfExperience = yearsOfExperience;
            PhoneNumber = phoneNumber;
            OfficeAddress = officeAddress;
            Education = education;
            Biography = biography;
            ConsultationFee = consultationFee;
            ConsultationDuration = consultationDuration ?? TimeSpan.FromMinutes(30);

            Appointments = new List<Appointment>();
            CreatedMedicalRecords = new List<MedicalRecord>();
            ModifiedMedicalRecords = new List<MedicalRecord>();
            Prescriptions = new List<Prescription>();
        }

        protected Doctor() : base()
        {
            Appointments = new List<Appointment>();
            CreatedMedicalRecords = new List<MedicalRecord>();
            ModifiedMedicalRecords = new List<MedicalRecord>();
            Prescriptions = new List<Prescription>();
        }

        // =========================
        // Business Methods
        // =========================
        public void AssignToHospital(Hospital hospital)
        {
            if (hospital == null)
                throw new ArgumentNullException(nameof(hospital));

            Hospital = hospital;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateExperience(int years)
        {
            if (years < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(years));

            YearsOfExperience = years;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateConsultationFee(decimal fee)
        {
            if (fee < 0)
                throw new ArgumentException("Consultation fee cannot be negative.", nameof(fee));

            ConsultationFee = fee;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateProfile(string education, string biography, string phoneNumber, string officeAddress)
        {
            Education = education;
            Biography = biography;
            PhoneNumber = phoneNumber;
            OfficeAddress = officeAddress;
            UpdatedDate = DateTime.UtcNow;
        }

        // =========================
        // Validation
        // =========================
        private void ValidateDoctorData(string name, string email, string licenseNumber, int yearsOfExperience)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Doctor name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("Email must be valid.", nameof(email));

            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number cannot be empty.", nameof(licenseNumber));

            if (yearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(yearsOfExperience));
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
