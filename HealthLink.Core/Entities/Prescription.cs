using System;

namespace HealthLink.Core.Entities
{
    public class Prescription : BaseEntity
    {
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
        public Guid PrescribedByDoctorId { get; private set; }

        public Prescription(Guid id, string medicationName, string dosage, string frequency,
                          int durationDays, Guid prescribedByDoctorId)
            : base(id)
        {
            ValidatePrescriptionData(medicationName, dosage, frequency, durationDays);

            MedicationName = medicationName;
            Dosage = dosage;
            Frequency = frequency;
            DurationDays = durationDays;
            PrescribedByDoctorId = prescribedByDoctorId;
            StartDate = DateTime.UtcNow;
            EndDate = DateTime.UtcNow.AddDays(durationDays);
            Quantity = 1;
        }

        protected Prescription() : base() { }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

            Quantity = quantity;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }

        private void ValidatePrescriptionData(string medicationName, string dosage, string frequency, int durationDays)
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