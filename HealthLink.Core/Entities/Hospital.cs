namespace HealthLink.Core.Entities
{
    /// Represents a hospital or healthcare facility.
    /// Hospitals employ doctors and provide medical services.
    public class Hospital : BaseEntity
    {
        public string Name { get; private set; }
        public string RegistrationNumber { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string PhoneNumber { get; private set; }

        // One Hospital has many Doctors
        public virtual ICollection<Doctor> Doctors { get; private set; }

        // Constructor
        public Hospital(Guid id, string name, string registrationNumber, string address,
                       string city, string phoneNumber)
            : base(id)
        {
            ValidateHospitalData(name, registrationNumber);

            Name = name;
            RegistrationNumber = registrationNumber;
            Address = address;
            City = city;
            PhoneNumber = phoneNumber;

            // Initialize collections
            Doctors = new List<Doctor>();
        }

        // Parameterless constructor for EF Core
        protected Hospital() : base()
        {
            Doctors = new List<Doctor>();
        }

        // Business Methods
        public void UpdateContactInfo(string address, string city, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty.", nameof(address));

            Address = address;
            City = city;
            PhoneNumber = phoneNumber;
            UpdatedDate = DateTime.UtcNow;
        }

        public void HireDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            if (Doctors.Any(d => d.Id == doctor.Id))
                throw new InvalidOperationException("This doctor is already employed by this hospital.");

            Doctors.Add(doctor);
            doctor.AssignToHospital(this);
            UpdatedDate = DateTime.UtcNow;
        }

        private void ValidateHospitalData(string name, string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hospital name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(registrationNumber))
                throw new ArgumentException("Registration number cannot be empty.", nameof(registrationNumber));
        }
    }
}