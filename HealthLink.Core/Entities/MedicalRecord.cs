using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthLink.Core.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid CreatedByDoctorId { get; private set; }
        public Guid? ModifiedByDoctorId { get; private set; }
        public string Diagnosis { get; private set; }
        public string Symptoms { get; private set; }
        public string TestsRecommended { get; private set; }
        public string PhysicalExamination { get; private set; }
        public string Treatment { get; private set; }
        public string Notes { get; private set; }

        private List<Prescription> _prescriptions = new();
        public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();

        public MedicalRecord(Guid id, Guid patientId, Guid createdByDoctorId,
                            string diagnosis, string symptoms)
            : base(id)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnosis cannot be empty.", nameof(diagnosis));

            if (string.IsNullOrWhiteSpace(symptoms))
                throw new ArgumentException("Symptoms cannot be empty.", nameof(symptoms));

            PatientId = patientId;
            CreatedByDoctorId = createdByDoctorId;
            Diagnosis = diagnosis;
            Symptoms = symptoms;
        }

        protected MedicalRecord() : base() { }

        public void AddPrescription(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription));

            if (_prescriptions.Any(p => p.Id == prescription.Id))
                throw new InvalidOperationException("This prescription is already added to the record.");

            _prescriptions.Add(prescription);
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateNotes(string newNotes)
        {
            Notes = newNotes;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}