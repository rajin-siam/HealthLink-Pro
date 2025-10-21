using Xunit;
using HealthLink.Core.Entities;
using System;

namespace HealthLink.Tests.Entities
{
    public class MedicalRecordTests
    {
        [Fact]
        public void CreateMedicalRecord_WithValidData_ShouldSucceed()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var diagnosis = "Hypertension";
            var symptoms = "High blood pressure";

            // Act
            var record = new MedicalRecord(
                id: recordId,
                patientId: patientId,
                createdByDoctorId: doctorId,
                diagnosis: diagnosis,
                symptoms: symptoms
            );

            // Assert
            Assert.Equal(recordId, record.Id);
            Assert.Equal(patientId, record.PatientId);
            Assert.Equal(doctorId, record.CreatedByDoctorId);
            Assert.Equal(diagnosis, record.Diagnosis);
            Assert.Equal(symptoms, record.Symptoms);
        }

        [Fact]
        public void MedicalRecord_CanAddPrescription()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                patientId: Guid.NewGuid(),
                createdByDoctorId: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High BP"
            );

            var prescription = new Prescription(
                id: Guid.NewGuid(),
                medicationName: "Lisinopril",
                dosage: "10mg",
                frequency: "Once daily",
                durationDays: 30,
                prescribedByDoctorId: record.CreatedByDoctorId
            );

            // Act
            record.AddPrescription(prescription);

            // Assert
            Assert.Contains(prescription, record.Prescriptions);
        }

        [Fact]
        public void MedicalRecord_CannotHaveDuplicatePrescriptions()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                patientId: Guid.NewGuid(),
                createdByDoctorId: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High BP"
            );

            var prescriptionId = Guid.NewGuid();
            var prescription1 = new Prescription(
                id: prescriptionId,
                medicationName: "Lisinopril",
                dosage: "10mg",
                frequency: "Once daily",
                durationDays: 30,
                prescribedByDoctorId: record.CreatedByDoctorId
            );

            record.AddPrescription(prescription1);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                record.AddPrescription(prescription1)
            );

            Assert.Contains("already", exception.Message.ToLower());
        }
    }
}