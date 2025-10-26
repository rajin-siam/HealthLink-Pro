using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents a medical record containing diagnosis, symptoms, vitals, and treatment information.
    /// Created by doctors for patients and can contain multiple prescriptions.
    /// </summary>
    public class MedicalRecord : BaseEntity
    {
        // =========================
        // Core Clinical Information
        // =========================
        public string Diagnosis { get; private set; }
        public string Symptoms { get; private set; }
        public string ChiefComplaint { get; private set; }        // "Chest pain", "Shortness of breath"
        public string PhysicalExamination { get; private set; }
        public string TestsRecommended { get; private set; }
        public string LabResults { get; private set; }
        public string ImagingResults { get; private set; }
        public string Treatment { get; private set; }
        public string Notes { get; private set; }

        // =========================
        // Vital Signs (Structured)
        // =========================
        public decimal? Temperature { get; private set; }         // e.g., 37.5°C
        public string BloodPressure { get; private set; }         // e.g., "120/80"
        public int? HeartRate { get; private set; }               // bpm
        public int? RespirationRate { get; private set; }         // breaths per minute
        public decimal? OxygenSaturation { get; private set; }    // e.g., 98%

        // =========================
        // Follow-Up & Classification
        // =========================
        public DateTime? NextFollowUpDate { get; private set; }
        public string FollowUpInstructions { get; private set; }
        public string RecordType { get; private set; }            // "Routine", "Emergency", etc.
        public bool IsConfidential { get; private set; } = false; // Sensitive data flag

        // =========================
        // Relationships
        // =========================
        public virtual Patient Patient { get; private set; }          // Belongs to one patient
        public virtual Doctor CreatedByDoctor { get; private set; }   // Created by one doctor
        public virtual Doctor ModifiedByDoctor { get; private set; }  // Modified by (optional) another doctor
        public virtual ICollection<Prescription> Prescriptions { get; private set; }

        // =========================
        // Constructors
        // =========================
        public MedicalRecord(Guid id, string diagnosis, string symptoms, string chiefComplaint = null)
            : base(id)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnosis cannot be empty.", nameof(diagnosis));

            if (string.IsNullOrWhiteSpace(symptoms))
                throw new ArgumentException("Symptoms cannot be empty.", nameof(symptoms));

            Diagnosis = diagnosis;
            Symptoms = symptoms;
            ChiefComplaint = chiefComplaint;

            Prescriptions = new List<Prescription>();
        }

        protected MedicalRecord() : base()
        {
            Prescriptions = new List<Prescription>();
        }

        // =========================
        // Business Methods
        // =========================

        public void AssignPatient(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetCreatingDoctor(Doctor doctor)
        {
            CreatedByDoctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateMedicalInfo(
            Doctor modifyingDoctor,
            string diagnosis = null,
            string symptoms = null,
            string treatment = null,
            string notes = null)
        {
            if (modifyingDoctor == null)
                throw new ArgumentNullException(nameof(modifyingDoctor));

            if (!string.IsNullOrWhiteSpace(diagnosis)) Diagnosis = diagnosis;
            if (!string.IsNullOrWhiteSpace(symptoms)) Symptoms = symptoms;
            if (!string.IsNullOrWhiteSpace(treatment)) Treatment = treatment;
            if (!string.IsNullOrWhiteSpace(notes)) Notes = notes;

            ModifiedByDoctor = modifyingDoctor;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetVitalSigns(decimal? temp, string bp, int? hr, int? rr, decimal? o2)
        {
            Temperature = temp;
            BloodPressure = bp;
            HeartRate = hr;
            RespirationRate = rr;
            OxygenSaturation = o2;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetTestsAndExams(string tests, string examination)
        {
            TestsRecommended = tests;
            PhysicalExamination = examination;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddLabAndImagingResults(string labResults, string imagingResults)
        {
            LabResults = labResults;
            ImagingResults = imagingResults;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddPrescription(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription));

            if (Prescriptions.Any(p => p.Id == prescription.Id))
                throw new InvalidOperationException("This prescription is already linked to this record.");

            Prescriptions.Add(prescription);
            UpdatedDate = DateTime.UtcNow;
        }

        public void ScheduleFollowUp(DateTime date, string instructions)
        {
            if (date <= DateTime.UtcNow)
                throw new ArgumentException("Follow-up date must be in the future.", nameof(date));

            NextFollowUpDate = date;
            FollowUpInstructions = instructions;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsConfidential(bool isConfidential = true)
        {
            IsConfidential = isConfidential;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetRecordType(string recordType)
        {
            if (string.IsNullOrWhiteSpace(recordType))
                throw new ArgumentException("Record type cannot be empty.", nameof(recordType));

            RecordType = recordType;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateNotes(string newNotes)
        {
            Notes = newNotes;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}
