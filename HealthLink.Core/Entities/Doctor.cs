using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents a doctor in the healthcare system.
    /// Doctors can create medical records, prescribe medications, and have appointments.
    /// </summary>
    public class Doctor : BaseEntity
    {
        // Basic Information
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string LicenseNumber { get; private set; }
        public Specialization Specialization { get; private set; }
        public int YearsOfExperience { get; private set; }


        public virtual Hospital Hospital { get; set; } 
        public virtual ICollection<Appointment> Appointments { get; private set; }
        public virtual ICollection<MedicalRecord> CreatedMedicalRecords { get; private set; }
        public virtual ICollection<MedicalRecord> ModifiedMedicalRecords { get; private set; }
        public virtual ICollection<Prescription> Prescriptions { get; private set; }


        public Doctor(Guid id, string name, string email, string licenseNumber,
                      Specialization specialization, int yearsOfExperience)
            : base(id)
        {
            ValidateDoctorData(name, email, licenseNumber, yearsOfExperience);

            Name = name;
            Email = email;
            LicenseNumber = licenseNumber;
            Specialization = specialization;
            YearsOfExperience = yearsOfExperience;

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

        private void ValidateDoctorData(string name, string email, string licenseNumber, int yearsOfExperience)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Doctor name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("Email must be a valid email address.", nameof(email));

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