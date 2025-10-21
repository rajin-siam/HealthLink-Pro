using HealthLink.Core.Validators;

namespace HealthLink.Core.Entities
{
    /// Represents a patient in the healthcare system.
    /// Contains basic patient information and navigations to related medical data.
    public class Patient : BaseEntity
    {
        // Basic Information
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string BloodType { get; private set; }
        public decimal Height { get; private set; } // in cm
        public decimal Weight { get; private set; }  // in kg



        public virtual ICollection<Allergy> Allergies { get; private set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; private set; }
        public virtual ICollection<Appointment> Appointments { get; private set; }


        public Patient(Guid id, string name, string email, string bloodType, decimal height, decimal weight)
            : base(id)
        {
            ValidateConstructorParameters(name, email, bloodType, height, weight);

            Name = name;
            Email = email;
            BloodType = bloodType;
            Height = height;
            Weight = weight;

            Allergies = new List<Allergy>();
            MedicalRecords = new List<MedicalRecord>();
            Appointments = new List<Appointment>();
        }

        protected Patient() : base()
        {
            // Initialize collections even in parameterless constructor
            Allergies = new List<Allergy>();
            MedicalRecords = new List<MedicalRecord>();
            Appointments = new List<Appointment>();
        }

        // Business Methods
        public void UpdatePersonalInfo(string name, string email, decimal height, decimal weight)
        {
            ValidateConstructorParameters(name, email, BloodType, height, weight);

            Name = name;
            Email = email;
            Height = height;
            Weight = weight;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddAllergy(Allergy allergy)
        {
            if (allergy == null)
                throw new ArgumentNullException(nameof(allergy));

            if (Allergies.Any(a => a.Id == allergy.Id))
                throw new InvalidOperationException("Patient already has this allergy.");

            Allergies.Add(allergy);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveAllergy(Guid allergyId)
        {
            var allergy = Allergies.FirstOrDefault(a => a.Id == allergyId);
            if (allergy != null)
            {
                Allergies.Remove(allergy);
                UpdatedDate = DateTime.UtcNow;
            }
        }

        private void ValidateConstructorParameters(string name, string email, string bloodType, decimal height, decimal weight)
        {
            PatientValidator.ValidatePatientData(name, email, bloodType, height, weight);
        }
    }
}