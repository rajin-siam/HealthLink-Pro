using HealthLink.Core.Entities;
using HealthLink.Core.Enums;
using System;
using System.Numerics;
using Xunit;

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
    }
}