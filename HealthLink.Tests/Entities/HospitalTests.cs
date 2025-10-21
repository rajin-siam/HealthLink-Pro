using Xunit;
using HealthLink.Core.Entities;

namespace HealthLink.Tests.Entities
{
    public class HospitalTests
    {
        [Fact]
        public void CreateHospital_WithValidData_ShouldSucceed()
        {
            // Arrange
            var hospitalId = Guid.NewGuid();
            var name = "City Hospital";
            var registrationNumber = "REG123";
            var address = "123 Main St";
            var city = "New York";
            var phoneNumber = "+1234567890";

            // Act
            var hospital = new Hospital(
                id: hospitalId,
                name: name,
                registrationNumber: registrationNumber,
                address: address,
                city: city,
                phoneNumber: phoneNumber
            );

            // Assert
            Assert.Equal(hospitalId, hospital.Id);
            Assert.Equal(name, hospital.Name);
            Assert.Equal(city, hospital.City);
        }

        [Fact]
        public void CreateHospital_WithoutName_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Hospital(
                    id: Guid.NewGuid(),
                    name: "",
                    registrationNumber: "REG123",
                    address: "123 Main St",
                    city: "New York",
                    phoneNumber: "+1234567890"
                )
            );

            Assert.Contains("name", exception.Message.ToLower());
        }
    }
}