using Xunit;
using HealthLink.Core.Entities;


namespace HealthLink.Tests.Entities
{
    public class MedicalRecordTests
    {
        [Fact]
        public void CreateMedicalRecord_WithValidData_ShouldSucceed()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            var diagnosis = "Hypertension";
            var symptoms = "High blood pressure, headaches";

            // Act
            var record = new MedicalRecord(
                id: recordId,
                diagnosis: diagnosis,
                symptoms: symptoms
            );

            // Assert
            Assert.Equal(recordId, record.Id);
            Assert.Equal(diagnosis, record.Diagnosis);
            Assert.Equal(symptoms, record.Symptoms);
            Assert.NotNull(record.Prescriptions);
            Assert.Empty(record.Prescriptions);
        }

        [Fact]
        public void CreateMedicalRecord_WithEmptyDiagnosis_ShouldThrow()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            var emptyDiagnosis = "";
            var symptoms = "High blood pressure";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new MedicalRecord(
                    id: recordId,
                    diagnosis: emptyDiagnosis,
                    symptoms: symptoms
                )
            );

            Assert.Contains("diagnosis", exception.Message.ToLower());
        }

        [Fact]
        public void CreateMedicalRecord_WithEmptySymptoms_ShouldThrow()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            var diagnosis = "Hypertension";
            var emptySymptoms = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new MedicalRecord(
                    id: recordId,
                    diagnosis: diagnosis,
                    symptoms: emptySymptoms
                )
            );

            Assert.Contains("symptoms", exception.Message.ToLower());
        }

        [Fact]
        public void CreateMedicalRecord_WithNullDiagnosis_ShouldThrow()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            string nullDiagnosis = null;
            var symptoms = "High blood pressure";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new MedicalRecord(
                    id: recordId,
                    diagnosis: nullDiagnosis,
                    symptoms: symptoms
                )
            );

            Assert.Contains("diagnosis", exception.Message.ToLower());
        }

        [Fact]
        public void CreateMedicalRecord_WithNullSymptoms_ShouldThrow()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            var diagnosis = "Hypertension";
            string nullSymptoms = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new MedicalRecord(
                    id: recordId,
                    diagnosis: diagnosis,
                    symptoms: nullSymptoms
                )
            );

            Assert.Contains("symptoms", exception.Message.ToLower());
        }

        [Fact]
        public void MedicalRecord_AssignPatient_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John Doe",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.AssignPatient(patient);

            // Assert
            Assert.Equal(patient, record.Patient);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_AssignNullPatient_ShouldThrow()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                record.AssignPatient(null)
            );
        }

        [Fact]
        public void MedicalRecord_SetCreatingDoctor_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Core.Enums.Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.SetCreatingDoctor(doctor);

            // Assert
            Assert.Equal(doctor, record.CreatedByDoctor);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_SetNullCreatingDoctor_ShouldThrow()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                record.SetCreatingDoctor(null)
            );
        }

        [Fact]
        public void MedicalRecord_UpdateMedicalInfo_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Initial Diagnosis",
                symptoms: "Initial Symptoms"
            );

            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Core.Enums.Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var newDiagnosis = "Updated Diagnosis";
            var newSymptoms = "Updated Symptoms";
            var newTreatment = "New treatment plan";
            var newNotes = "Important notes";

            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.UpdateMedicalInfo(
                modifyingDoctor: doctor,
                diagnosis: newDiagnosis,
                symptoms: newSymptoms,
                treatment: newTreatment,
                notes: newNotes
            );

            // Assert
            Assert.Equal(newDiagnosis, record.Diagnosis);
            Assert.Equal(newSymptoms, record.Symptoms);
            Assert.Equal(newTreatment, record.Treatment);
            Assert.Equal(newNotes, record.Notes);
            Assert.Equal(doctor, record.ModifiedByDoctor);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_UpdateMedicalInfoWithNullDoctor_ShouldThrow()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                record.UpdateMedicalInfo(null, "New Diagnosis")
            );
        }

        [Fact]
        public void MedicalRecord_UpdateMedicalInfoWithPartialData_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Initial Diagnosis",
                symptoms: "Initial Symptoms"
            );

            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Core.Enums.Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var originalDiagnosis = record.Diagnosis;
            var originalSymptoms = record.Symptoms;

            // Act - Only update treatment and notes
            record.UpdateMedicalInfo(
                modifyingDoctor: doctor,
                treatment: "New treatment",
                notes: "New notes"
            );

            // Assert
            Assert.Equal(originalDiagnosis, record.Diagnosis); // Should remain unchanged
            Assert.Equal(originalSymptoms, record.Symptoms);   // Should remain unchanged
            Assert.Equal("New treatment", record.Treatment);
            Assert.Equal("New notes", record.Notes);
            Assert.Equal(doctor, record.ModifiedByDoctor);
        }

        [Fact]
        public void MedicalRecord_SetTestsRecommended_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            var tests = "Blood test, ECG, Chest X-ray";
            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.SetTestsRecommended(tests);

            // Assert
            Assert.Equal(tests, record.TestsRecommended);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_SetPhysicalExamination_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            var examination = "BP: 140/90, Heart rate: 80 bpm";
            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.SetPhysicalExamination(examination);

            // Assert
            Assert.Equal(examination, record.PhysicalExamination);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_UpdateNotes_ShouldSucceed()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High blood pressure"
            );

            var newNotes = "Patient needs follow-up in 2 weeks";
            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.UpdateNotes(newNotes);

            // Assert
            Assert.Equal(newNotes, record.Notes);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_CanAddPrescription()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High BP"
            );

            var prescription = new Prescription(
                id: Guid.NewGuid(),
                medicationName: "Lisinopril",
                dosage: "10mg",
                frequency: "Once daily",
                durationDays: 30
            );

            var originalUpdatedDate = record.UpdatedDate;

            // Act
            record.AddPrescription(prescription);

            // Assert
            Assert.Contains(prescription, record.Prescriptions);
            Assert.Single(record.Prescriptions);
            Assert.True(record.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void MedicalRecord_AddNullPrescription_ShouldThrow()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High BP"
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                record.AddPrescription(null)
            );
        }

        [Fact]
        public void MedicalRecord_CannotHaveDuplicatePrescriptions()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High BP"
            );

            var prescriptionId = Guid.NewGuid();
            var prescription1 = new Prescription(
                id: prescriptionId,
                medicationName: "Lisinopril",
                dosage: "10mg",
                frequency: "Once daily",
                durationDays: 30
            );

            var prescription2 = new Prescription(
                id: prescriptionId, // Same ID
                medicationName: "Lisinopril",
                dosage: "10mg",
                frequency: "Once daily",
                durationDays: 30
            );

            record.AddPrescription(prescription1);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                record.AddPrescription(prescription2)
            );

            Assert.Contains("already", exception.Message.ToLower());
        }

        [Fact]
        public void MedicalRecord_CanAddMultipleDifferentPrescriptions()
        {
            // Arrange
            var record = new MedicalRecord(
                id: Guid.NewGuid(),
                diagnosis: "Hypertension",
                symptoms: "High BP"
            );

            var prescription1 = new Prescription(
                id: Guid.NewGuid(),
                medicationName: "Lisinopril",
                dosage: "10mg",
                frequency: "Once daily",
                durationDays: 30
            );

            var prescription2 = new Prescription(
                id: Guid.NewGuid(),
                medicationName: "Amlodipine",
                dosage: "5mg",
                frequency: "Once daily",
                durationDays: 30
            );

            // Act
            record.AddPrescription(prescription1);
            record.AddPrescription(prescription2);

            // Assert
            Assert.Contains(prescription1, record.Prescriptions);
            Assert.Contains(prescription2, record.Prescriptions);
            Assert.Equal(2, record.Prescriptions.Count);
        }
    }
}