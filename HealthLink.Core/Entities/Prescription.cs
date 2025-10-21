namespace HealthLink.Core.Entities
{
    /// Represents a prescription for medication.
    /// Belongs to a medical record and is prescribed by a doctor.
    public class Prescription : BaseEntity
    {
        // Medication Information
        public string MedicationName { get; private set; }
        public string GenericName { get; private set; }
        public string Dosage { get; private set; } // e.g., "500mg"
        public string Frequency { get; private set; } // e.g., "Twice daily"
        public int DurationDays { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Quantity { get; private set; }
        public string Instructions { get; private set; }
        public string Warnings { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Navigation Properties
        public virtual MedicalRecord MedicalRecord { get; set; } // Many-to-One: Prescription belongs to one MedicalRecord
        public virtual Doctor PrescribedByDoctor { get; set; }   // Many-to-One: Prescription prescribed by one Doctor

        // Constructor
        public Prescription(Guid id, string medicationName, string dosage,
            string frequency, int durationDays)
            : base(id)
        {
            ValidatePrescriptionData(medicationName, dosage, frequency, durationDays);

            MedicationName = medicationName;
            Dosage = dosage;
            Frequency = frequency;
            DurationDays = durationDays;
            StartDate = DateTime.UtcNow;
            EndDate = DateTime.UtcNow.AddDays(durationDays);
            Quantity = 1;
        }

        // Parameterless constructor for EF Core
        protected Prescription() : base() { }

        // Business Methods
        public void AssignToMedicalRecord(MedicalRecord medicalRecord)
        {
            if (medicalRecord == null)
                throw new ArgumentNullException(nameof(medicalRecord));

            MedicalRecord = medicalRecord;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetPrescribingDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            PrescribedByDoctor = doctor;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetGenericName(string genericName)
        {
            GenericName = genericName;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

            Quantity = quantity;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetInstructions(string instructions)
        {
            Instructions = instructions;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetWarnings(string warnings)
        {
            Warnings = warnings;
            UpdatedDate = DateTime.UtcNow;
        }

        public void ExtendDuration(int additionalDays)
        {
            if (additionalDays <= 0)
                throw new ArgumentException("Additional days must be greater than 0.", nameof(additionalDays));

            DurationDays += additionalDays;
            EndDate = EndDate.AddDays(additionalDays);
            UpdatedDate = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (EndDate < DateTime.UtcNow)
                throw new InvalidOperationException("Cannot reactivate an expired prescription.");

            IsActive = true;
            UpdatedDate = DateTime.UtcNow;
        }

        private void ValidatePrescriptionData(string medicationName, string dosage,
            string frequency, int durationDays)
        {
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name cannot be empty.", nameof(medicationName));

            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage cannot be empty.", nameof(dosage));

            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Frequency cannot be empty.", nameof(frequency));

            if (durationDays <= 0)
                throw new ArgumentException("Duration must be greater than 0.", nameof(durationDays));
        }
    }
}