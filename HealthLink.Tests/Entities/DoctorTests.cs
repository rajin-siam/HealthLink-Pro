
using Xunit;
using HealthLink.Core.Entities;
using HealthLink.Core.Enums;
using System;
using System.Linq;

namespace HealthLink.Tests.Entities
{
    public class DoctorTests
    {
        [Fact]
        public void CreateDoctor_WithValidData_ShouldSucceed()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var name = "Dr. Smith";
            var email = "smith@hospital.com";
            var licenseNumber = "LIC123456";
            var specialization = Specialization.Cardiology;
            var yearsOfExperience = 10;

            // Act
            var doctor = new Doctor(
                id: doctorId,
                name: name,
                email: email,
                licenseNumber: licenseNumber,
                specialization: specialization,
                yearsOfExperience: yearsOfExperience
            );

            // Assert
            Assert.Equal(doctorId, doctor.Id);
            Assert.Equal(name, doctor.Name);
            Assert.Equal(email, doctor.Email);
            Assert.Equal(licenseNumber, doctor.LicenseNumber);
            Assert.Equal(specialization, doctor.Specialization);
            Assert.Equal(yearsOfExperience, doctor.YearsOfExperience);
        }

        [Fact]
        public void CreateDoctor_WithInvalidLicense_ShouldThrow()
        {
            // Arrange
            var invalidLicense = ""; // Empty license

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Doctor(
                    id: Guid.NewGuid(),
                    name: "Dr. Smith",
                    email: "smith@hospital.com",
                    licenseNumber: invalidLicense,
                    specialization: Specialization.Cardiology,
                    yearsOfExperience: 10
                )
            );

            Assert.Contains("license", exception.Message.ToLower());
        }

        [Fact]
        public void CreateDoctor_WithNullLicense_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Doctor(
                    id: Guid.NewGuid(),
                    name: "Dr. Smith",
                    email: "smith@hospital.com",
                    licenseNumber: null,
                    specialization: Specialization.Cardiology,
                    yearsOfExperience: 10
                )
            );

            Assert.Contains("license", exception.Message.ToLower());
        }

        [Fact]
        public void CreateDoctor_WithNegativeExperience_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Doctor(
                    id: Guid.NewGuid(),
                    name: "Dr. Smith",
                    email: "smith@hospital.com",
                    licenseNumber: "LIC123456",
                    specialization: Specialization.Cardiology,
                    yearsOfExperience: -5 // Invalid
                )
            );

            Assert.Contains("experience", exception.Message.ToLower());
        }

        [Fact]
        public void CreateDoctor_WithInvalidEmail_ShouldThrow()
        {
            // Arrange
            var invalidEmail = "not-an-email";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Doctor(
                    id: Guid.NewGuid(),
                    name: "Dr. Smith",
                    email: invalidEmail,
                    licenseNumber: "LIC123456",
                    specialization: Specialization.Cardiology,
                    yearsOfExperience: 10
                )
            );

            Assert.Contains("email", exception.Message.ToLower());
        }

        [Fact]
        public void CreateDoctor_WithEmptyName_ShouldThrow()
        {
            // Arrange
            var emptyName = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Doctor(
                    id: Guid.NewGuid(),
                    name: emptyName,
                    email: "smith@hospital.com",
                    licenseNumber: "LIC123456",
                    specialization: Specialization.Cardiology,
                    yearsOfExperience: 10
                )
            );

            Assert.Contains("name", exception.Message.ToLower());
        }

        [Fact]
        public void CreateDoctor_WithNullName_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Doctor(
                    id: Guid.NewGuid(),
                    name: null,
                    email: "smith@hospital.com",
                    licenseNumber: "LIC123456",
                    specialization: Specialization.Cardiology,
                    yearsOfExperience: 10
                )
            );

            Assert.Contains("name", exception.Message.ToLower());
        }

        [Fact]
        public void Doctor_ShouldHaveEmptyCollectionsOnCreation()
        {
            // Arrange & Act
            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );

            // Assert
            Assert.NotNull(doctor.Appointments);
            Assert.Empty(doctor.Appointments);

            Assert.NotNull(doctor.CreatedMedicalRecords);
            Assert.Empty(doctor.CreatedMedicalRecords);

            Assert.NotNull(doctor.ModifiedMedicalRecords);
            Assert.Empty(doctor.ModifiedMedicalRecords);

            Assert.NotNull(doctor.Prescriptions);
            Assert.Empty(doctor.Prescriptions);
        }

        [Fact]
        public void Doctor_AssignToHospital_ShouldSucceed()
        {
            // Arrange
            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );


            
            var hospital = new Hospital(
                id: Guid.NewGuid(),
                name: "City Hospital",
                registrationNumber: "REG123",
                address: "123 Main St",
                city: "New York",
                phoneNumber: "+1234567890"
            );

            var originalUpdatedDate = doctor.UpdatedDate;

            // Act
            doctor.AssignToHospital(hospital);

            // Assert
            Assert.Equal(hospital, doctor.Hospital);
            Assert.True(doctor.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Doctor_AssignToNullHospital_ShouldThrow()
        {
            // Arrange
            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                doctor.AssignToHospital(null)
            );
        }

        [Fact]
        public void Doctor_UpdateExperience_ShouldSucceed()
        {
            // Arrange
            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var newExperience = 15;
            var originalUpdatedDate = doctor.UpdatedDate;

            // Act
            doctor.UpdateExperience(newExperience);

            // Assert
            Assert.Equal(newExperience, doctor.YearsOfExperience);
            Assert.True(doctor.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Doctor_UpdateExperienceWithNegativeValue_ShouldThrow()
        {
            // Arrange
            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var negativeExperience = -5;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                doctor.UpdateExperience(negativeExperience)
            );

            Assert.Contains("negative", exception.Message.ToLower());
        }

        [Fact]
        public void Doctor_UpdateExperienceWithSameValue_ShouldUpdateTimestamp()
        {
            // Arrange
            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var sameExperience = 10;
            var originalUpdatedDate = doctor.UpdatedDate;

            // Act
            doctor.UpdateExperience(sameExperience);

            // Assert
            Assert.Equal(sameExperience, doctor.YearsOfExperience);
            Assert.True(doctor.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Doctor_AllSpecializations_ShouldBeTestable()
        {
            // Test that we can create doctors with different specializations
            var specializations = Enum.GetValues(typeof(Specialization));

            foreach (Specialization specialization in specializations)
            {
                // Arrange & Act
                var doctor = new Doctor(
                    id: Guid.NewGuid(),
                    name: $"Dr. {specialization}",
                    email: $"{specialization.ToString().ToLower()}@hospital.com",
                    licenseNumber: $"LIC{specialization}",
                    specialization: specialization,
                    yearsOfExperience: 5
                );

                // Assert
                Assert.Equal(specialization, doctor.Specialization);
            }
        }
    }
}
