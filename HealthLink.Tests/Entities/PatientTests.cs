using Xunit;
using HealthLink.Core.Entities;
using HealthLink.Core.Enums;

namespace HealthLink.Tests.Entities
{
    public class PatientTests
    {
        [Fact]
        public void CreatePatient_WithValidData_ShouldSucceed()
        {
            // Arrange (set up test data)
            var patientId = Guid.NewGuid();
            var name = "John Doe";
            var email = "john@example.com";
            var bloodType = "O+";
            var height = 180m; // in cm
            var weight = 75m;  // in kg

            // Act (perform the action)
            var patient = new Patient(
                id: patientId,
                name: name,
                email: email,
                bloodType: bloodType,
                height: height,
                weight: weight
            );

            // Assert (verify the result)
            Assert.Equal(patientId, patient.Id);
            Assert.Equal(name, patient.Name);
            Assert.Equal(email, patient.Email);
            Assert.Equal(bloodType, patient.BloodType);
            Assert.Equal(height, patient.Height);
            Assert.Equal(weight, patient.Weight);
        }

        [Fact]
        public void CreatePatient_WithInvalidEmail_ShouldThrow()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var invalidEmail = "not-an-email";
            var height = 180m;
            var weight = 75m;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient(
                    id: patientId,
                    name: "John",
                    email: invalidEmail,
                    bloodType: "O+",
                    height: height,
                    weight: weight
                )
            );

            Assert.Contains("email", exception.Message.ToLower());
        }

        [Fact]
        public void CreatePatient_WithInvalidHeight_ShouldThrow()
        {
            // Arrange - Height must be between 50cm and 300cm (based on typical validator rules)
            var patientId = Guid.NewGuid();
            var invalidHeight = 10m; // Too small

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient(
                    id: patientId,
                    name: "John",
                    email: "john@example.com",
                    bloodType: "O+",
                    height: invalidHeight,
                    weight: 75
                )
            );

            Assert.Contains("height", exception.Message.ToLower());
        }

        [Fact]
        public void CreatePatient_WithInvalidWeight_ShouldThrow()
        {
            // Arrange - Weight must be positive (based on typical validator rules)
            var patientId = Guid.NewGuid();
            var invalidWeight = -5m; // Negative weight

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient(
                    id: patientId,
                    name: "John",
                    email: "john@example.com",
                    bloodType: "O+",
                    height: 180,
                    weight: invalidWeight
                )
            );

            Assert.Contains("weight", exception.Message.ToLower());
        }

        [Fact]
        public void CreatePatient_WithInvalidBloodType_ShouldThrow()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var invalidBloodType = "XYZ"; // Invalid blood type

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient(
                    id: patientId,
                    name: "John",
                    email: "john@example.com",
                    bloodType: invalidBloodType,
                    height: 180,
                    weight: 75
                )
            );

            Assert.Contains("blood", exception.Message.ToLower());
        }

        [Fact]
        public void CreatePatient_WithEmptyName_ShouldThrow()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var emptyName = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient(
                    id: patientId,
                    name: emptyName,
                    email: "john@example.com",
                    bloodType: "O+",
                    height: 180,
                    weight: 75
                )
            );

            Assert.Contains("name", exception.Message.ToLower());
        }

        [Fact]
        public void Patient_ShouldHaveEmptyAllergiesOnCreation()
        {
            // Arrange & Act
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            // Assert
            Assert.NotNull(patient.Allergies);
            Assert.Empty(patient.Allergies);
        }

        [Fact]
        public void Patient_ShouldHaveEmptyMedicalRecordsOnCreation()
        {
            // Arrange & Act
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            // Assert
            Assert.NotNull(patient.MedicalRecords);
            Assert.Empty(patient.MedicalRecords);
        }

        [Fact]
        public void Patient_ShouldHaveEmptyAppointmentsOnCreation()
        {
            // Arrange & Act
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            // Assert
            Assert.NotNull(patient.Appointments);
            Assert.Empty(patient.Appointments);
        }

        [Fact]
        public void Patient_CanAddAllergy()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var allergy = new Allergy(
                id: Guid.NewGuid(),
                name: "Penicillin",
                severity: AllergySeverity.Severe
            );

            // Act
            patient.AddAllergy(allergy);

            // Assert
            Assert.Contains(allergy, patient.Allergies);
            Assert.Single(patient.Allergies);
        }

        [Fact]
        public void Patient_AddDuplicateAllergy_ShouldThrow()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var allergyId = Guid.NewGuid();
            var allergy = new Allergy(
                id: allergyId,
                name: "Penicillin",
                severity: AllergySeverity.Severe
            );

            patient.AddAllergy(allergy);

            var duplicateAllergy = new Allergy(
                id: allergyId, // Same ID
                name: "Penicillin",
                severity: AllergySeverity.Severe
            );

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                patient.AddAllergy(duplicateAllergy)
            );

            Assert.Contains("already has this allergy", exception.Message);
        }

        [Fact]
        public void Patient_AddNullAllergy_ShouldThrow()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                patient.AddAllergy(null)
            );
        }

        [Fact]
        public void Patient_RemoveAllergy_ShouldSucceed()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var allergyId = Guid.NewGuid();
            var allergy = new Allergy(
                id: allergyId,
                name: "Penicillin",
                severity: AllergySeverity.Severe
            );

            patient.AddAllergy(allergy);

            // Act
            patient.RemoveAllergy(allergyId);

            // Assert
            Assert.DoesNotContain(allergy, patient.Allergies);
            Assert.Empty(patient.Allergies);
        }

        [Fact]
        public void Patient_RemoveNonExistentAllergy_ShouldNotThrow()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var nonExistentAllergyId = Guid.NewGuid();

            // Act & Assert
            // Should not throw when removing non-existent allergy
            patient.RemoveAllergy(nonExistentAllergyId);
            Assert.Empty(patient.Allergies);
        }

        [Fact]
        public void UpdatePersonalInfo_WithValidData_ShouldSucceed()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var newName = "John Smith";
            var newEmail = "john.smith@example.com";
            var newHeight = 185m;
            var newWeight = 80m;

            var originalUpdatedDate = patient.UpdatedDate;

            // Act
            patient.UpdatePersonalInfo(newName, newEmail, newHeight, newWeight);

            // Assert
            Assert.Equal(newName, patient.Name);
            Assert.Equal(newEmail, patient.Email);
            Assert.Equal(newHeight, patient.Height);
            Assert.Equal(newWeight, patient.Weight);
            Assert.True(patient.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void UpdatePersonalInfo_WithInvalidData_ShouldThrow()
        {
            // Arrange
            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var invalidEmail = "not-an-email";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                patient.UpdatePersonalInfo("John", invalidEmail, 180, 75)
            );
        }
    }
}