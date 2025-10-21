using HealthLink.Core.Validators;

namespace HealthLink.Core.Entities
{
    public class Patient : BaseEntity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string BloodType { get; private set; }
        public decimal Height { get; private set; } // in cm
        public decimal Weight { get; private set; }  // in kg

        // Navigation property
        private List<Allergy> _allergies = new();
        public IReadOnlyCollection<Allergy> Allergies => _allergies.AsReadOnly();

        // Constructor
        public Patient(Guid id, string name, string email, string bloodType, decimal height, decimal weight) : base(id)
        {
            ValidateConstructorParameters(name, email, bloodType, height, weight);

            Id = id;
            Name = name;
            Email = email;
            BloodType = bloodType;
            Height = height;
            Weight = weight;
        }

        // Parameterless constructor for Entity Framework
        protected Patient(): base() { }

        // Methods
        public void AddAllergy(Allergy allergy)
        {
            if (allergy == null)
                throw new ArgumentNullException(nameof(allergy));

            if (_allergies.Any(a => a.Id == allergy.Id))
                throw new InvalidOperationException("Patient already has this allergy.");

            _allergies.Add(allergy);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveAllergy(Guid allergyId)
        {
            var allergy = _allergies.FirstOrDefault(a => a.Id == allergyId);
            if (allergy != null)
            {
                _allergies.Remove(allergy);
                UpdatedDate = DateTime.UtcNow;
            }
        }

        private void ValidateConstructorParameters(string name, string email, string bloodType, decimal height, decimal weight)
        {
            PatientValidator.ValidatePatientData(name, email, bloodType, height, weight);
        }
    }
}