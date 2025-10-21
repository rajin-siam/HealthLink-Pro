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

            Assert.Contains("valid email", exception.Message);
        }

        [Fact]
        public void CreatePatient_WithInvalidHeight_ShouldThrow()
        {
            // Arrange - Height must be between 50cm and 300cm
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
    }
}