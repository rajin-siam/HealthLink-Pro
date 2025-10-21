using Xunit;
using HealthLink.Core.Entities;
using FluentAssertions;

namespace HealthLink.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_WithValidData_ShouldSucceed()
        {
            // Arrange
            var userName = "johndoe";
            var email = "john@example.com";
            var fullName = "John Doe";

            // Act
            var user = new User(userName, email, fullName);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBeEmpty();
            user.UserName.Should().Be(userName);
            user.Email.Should().Be(email);
            user.FullName.Should().Be(fullName);
            user.IsActive.Should().BeTrue();
            user.EmailConfirmed.Should().BeFalse();
            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateUser_WithNullUserName_ShouldThrow()
        {
            // Arrange
            string userName = null;
            var email = "john@example.com";
            var fullName = "John Doe";

            // Act
            Action act = () => new User(userName, email, fullName);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*userName*");
        }

        [Fact]
        public void CreateUser_WithNullEmail_ShouldThrow()
        {
            // Arrange
            var userName = "johndoe";
            string email = null;
            var fullName = "John Doe";

            // Act
            Action act = () => new User(userName, email, fullName);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*email*");
        }

        [Fact]
        public void UpdateProfile_WithValidData_ShouldSucceed()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var newFullName = "John Smith";
            var newEmail = "johnsmith@example.com";

            // Act
            user.UpdateProfile(newFullName, newEmail);

            // Assert
            user.FullName.Should().Be(newFullName);
            user.Email.Should().Be(newEmail);
            user.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void LinkToPatient_WhenNotLinked_ShouldSucceed()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var patientId = Guid.NewGuid();

            // Act
            user.LinkToPatient(patientId);

            // Assert
            user.PatientId.Should().Be(patientId);
            user.DoctorId.Should().BeNull();
            user.HospitalId.Should().BeNull();
        }

        [Fact]
        public void LinkToPatient_WhenAlreadyLinkedToDoctor_ShouldThrow()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var doctorId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            user.LinkToDoctor(doctorId);

            // Act
            Action act = () => user.LinkToPatient(patientId);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*already linked*");
        }

        [Fact]
        public void LinkToDoctor_WhenNotLinked_ShouldSucceed()
        {
            // Arrange
            var user = new User("drsmith", "smith@hospital.com", "Dr. Smith");
            var doctorId = Guid.NewGuid();

            // Act
            user.LinkToDoctor(doctorId);

            // Assert
            user.DoctorId.Should().Be(doctorId);
            user.PatientId.Should().BeNull();
            user.HospitalId.Should().BeNull();
        }

        [Fact]
        public void LinkToHospital_WhenNotLinked_ShouldSucceed()
        {
            // Arrange
            var user = new User("cityhospital", "admin@city.com", "City Hospital");
            var hospitalId = Guid.NewGuid();

            // Act
            user.LinkToHospital(hospitalId);

            // Assert
            user.HospitalId.Should().Be(hospitalId);
            user.PatientId.Should().BeNull();
            user.DoctorId.Should().BeNull();
        }

        [Fact]
        public void RecordLogin_ShouldUpdateLastLoginDate()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            var beforeLogin = DateTime.UtcNow;

            // Act
            user.RecordLogin();

            // Assert
            user.LastLoginDate.Should().NotBeNull();
            user.LastLoginDate.Should().BeOnOrAfter(beforeLogin);
            user.LastLoginDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");

            // Act
            user.Deactivate();

            // Assert
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");
            user.Deactivate();

            // Act
            user.Activate();

            // Assert
            user.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "john@example.com")]
        [InlineData("   ", "john@example.com")]
        [InlineData("John Doe", "")]
        [InlineData("John Doe", "   ")]
        public void UpdateProfile_WithInvalidData_ShouldThrow(string fullName, string email)
        {
            // Arrange
            var user = new User("johndoe", "john@example.com", "John Doe");

            // Act
            Action act = () => user.UpdateProfile(fullName, email);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}