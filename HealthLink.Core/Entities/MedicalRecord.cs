namespace HealthLink.Core.Entities
{
    /// Represents a medical record containing diagnosis, symptoms, and treatment information.
    /// Created by doctors for patients and can contain multiple prescriptions.
    public class MedicalRecord : BaseEntity
    {
        // Medical Information
        public string Diagnosis { get; private set; }
        public string Symptoms { get; private set; }
        public string TestsRecommended { get; private set; }
        public string PhysicalExamination { get; private set; }
        public string Treatment { get; private set; }
        public string Notes { get; private set; }

        // Navigation Properties
        public virtual Patient Patient { get; set; } // Many-to-One: Record belongs to one Patient

        // One record is created by one doctor
        public virtual Doctor CreatedByDoctor { get; set; }

        // One record can be modified by another doctor (optional)
        public virtual Doctor ModifiedByDoctor { get; set; }

        // One medical record can have many prescriptions
        public virtual ICollection<Prescription> Prescriptions { get; private set; }

        // Constructor
        public MedicalRecord(Guid id, string diagnosis, string symptoms)
            : base(id)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnosis cannot be empty.", nameof(diagnosis));

            if (string.IsNullOrWhiteSpace(symptoms))
                throw new ArgumentException("Symptoms cannot be empty.", nameof(symptoms));

            Diagnosis = diagnosis;
            Symptoms = symptoms;

            Prescriptions = new List<Prescription>();
        }

        // Parameterless constructor for EF Core
        protected MedicalRecord() : base()
        {
            Prescriptions = new List<Prescription>();
        }

        // Business Methods
        public void AssignPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            Patient = patient;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetCreatingDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            CreatedByDoctor = doctor;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateMedicalInfo(Doctor modifyingDoctor, string diagnosis = null,
            string symptoms = null, string treatment = null, string notes = null)
        {
            if (modifyingDoctor == null)
                throw new ArgumentNullException(nameof(modifyingDoctor));

            if (!string.IsNullOrWhiteSpace(diagnosis))
                Diagnosis = diagnosis;

            if (!string.IsNullOrWhiteSpace(symptoms))
                Symptoms = symptoms;

            if (!string.IsNullOrWhiteSpace(treatment))
                Treatment = treatment;

            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes;

            ModifiedByDoctor = modifyingDoctor;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetTestsRecommended(string tests)
        {
            TestsRecommended = tests;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetPhysicalExamination(string examination)
        {
            PhysicalExamination = examination;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddPrescription(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription));

            if (Prescriptions.Any(p => p.Id == prescription.Id))
                throw new InvalidOperationException("This prescription is already added to the record.");

            Prescriptions.Add(prescription);
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateNotes(string newNotes)
        {
            Notes = newNotes;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}