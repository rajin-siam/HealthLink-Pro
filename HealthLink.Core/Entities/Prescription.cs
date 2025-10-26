namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents a prescription for medication.
    /// Belongs to a medical record and is prescribed by a doctor.
    /// </summary>
    public class Prescription : BaseEntity
    {
        // Medication Information
        public string MedicationName { get; private set; }
        public string GenericName { get; private set; }
        public string Dosage { get; private set; } // e.g., "500mg"
        public string Frequency { get; private set; } // e.g., "Twice daily"
        public string Route { get; private set; } // ✅ "Oral" vs "IV" vs "Topical"
        public int DurationDays { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Quantity { get; private set; }
        public string Instructions { get; private set; }
        public string Warnings { get; private set; }
        public string SideEffectsToWatch { get; private set; } // ✅ "Dizziness, nausea"
        public string Interactions { get; private set; } // ✅ "Don't take with alcohol"

        // Refill Management
        public int RefillsRemaining { get; private set; } = 0; // ✅ 2 refills left
        public int TotalRefills { get; private set; } = 0; // ✅ Originally 3 refills
        public DateTime? LastRefillDate { get; private set; } // ✅ Last refilled Jan 15

        // Pharmacy Information
        public string PharmacyName { get; private set; } // ✅ "CVS Pharmacy"
        public string PharmacyPhone { get; private set; } // ✅ "555-0123"

        // Follow-up & Status
        public bool RequiresFollowUp { get; private set; } // ✅ Need to check back
        public DateTime? FollowUpDate { get; private set; } // ✅ Check liver in 30 days
        public PrescriptionStatus Status { get; private set; } // ✅ Active/Completed/Discontinued
        public bool IsActive { get; private set; } = true;

        // Navigation Properties
        public virtual MedicalRecord MedicalRecord { get; set; } // Many-to-One: Prescription belongs to one MedicalRecord
        public virtual Doctor PrescribedByDoctor { get; set; } // Many-to-One: Prescription prescribed by one Doctor

        // Constructor
        public Prescription(Guid id, string medicationName, string dosage,
            string frequency, int durationDays, int totalRefills = 0)
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
            TotalRefills = totalRefills;
            RefillsRemaining = totalRefills;
            Status = PrescriptionStatus.Active;
        }

        // Parameterless constructor for EF Core
        protected Prescription() : base() { }

        // Business Methods - Original
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

        // Business Methods - New
        public void ProcessRefill()
        {
            if (RefillsRemaining <= 0)
                throw new InvalidOperationException("No refills remaining.");

            RefillsRemaining--;
            LastRefillDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Complete()
        {
            Status = PrescriptionStatus.Completed;
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Discontinue()
        {
            Status = PrescriptionStatus.Discontinued;
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }

        public void PutOnHold()
        {
            Status = PrescriptionStatus.OnHold;
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetPharmacyInfo(string pharmacyName, string pharmacyPhone)
        {
            if (string.IsNullOrWhiteSpace(pharmacyName))
                throw new ArgumentException("Pharmacy name cannot be empty.", nameof(pharmacyName));

            PharmacyName = pharmacyName;
            PharmacyPhone = pharmacyPhone;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetSafetyInfo(string sideEffects, string interactions, string route)
        {
            SideEffectsToWatch = sideEffects;
            Interactions = interactions;
            Route = route;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetFollowUp(bool requiresFollowUp, DateTime? followUpDate = null)
        {
            RequiresFollowUp = requiresFollowUp;
            FollowUpDate = followUpDate;
            UpdatedDate = DateTime.UtcNow;
        }

        // Updated Deactivate/Reactivate to handle status properly
        public void Deactivate()
        {
            IsActive = false;
            if (Status == PrescriptionStatus.Active)
            {
                Status = PrescriptionStatus.Discontinued;
            }
            UpdatedDate = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (EndDate < DateTime.UtcNow)
                throw new InvalidOperationException("Cannot reactivate an expired prescription.");

            IsActive = true;
            Status = PrescriptionStatus.Active;
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

    public enum PrescriptionStatus
    {
        Active = 1, // ✅ Currently taking
        Completed = 2, // ✅ Finished treatment
        Discontinued = 3, // ✅ Stopped due to side effects
        OnHold = 4 // ✅ Temporarily paused
    }
}