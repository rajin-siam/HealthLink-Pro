using Xunit;
using HealthLink.Core.Entities;
using HealthLink.Core.Enums;
using System;

namespace HealthLink.Tests.Entities
{
    public class AppointmentTests
    {
        [Fact]
        public void CreateAppointment_WithValidData_ShouldSucceed()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var appointmentTime = DateTime.UtcNow.AddDays(7); // Next week

            // Act
            var appointment = new Appointment(
                id: appointmentId,
                patientId: patientId,
                doctorId: doctorId,
                appointmentDateTime: appointmentTime
            );

            // Assert
            Assert.Equal(appointmentId, appointment.Id);
            Assert.Equal(patientId, appointment.PatientId);
            Assert.Equal(doctorId, appointment.DoctorId);
            Assert.Equal(appointmentTime, appointment.AppointmentDateTime);
            Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        }

        [Fact]
        public void CreateAppointment_WithPastDateTime_ShouldThrow()
        {
            // Arrange
            var pastTime = DateTime.UtcNow.AddDays(-1); // Yesterday

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Appointment(
                    id: Guid.NewGuid(),
                    patientId: Guid.NewGuid(),
                    doctorId: Guid.NewGuid(),
                    appointmentDateTime: pastTime
                )
            );

            Assert.Contains("future", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_CanBeCompleted()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                patientId: Guid.NewGuid(),
                doctorId: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Act
            appointment.MarkAsCompleted("Consultation completed successfully");

            // Assert
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);
            Assert.NotNull(appointment.Notes);
        }

        [Fact]
        public void Appointment_CanBeCancelled()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                patientId: Guid.NewGuid(),
                doctorId: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Act
            appointment.Cancel("Doctor emergency");

            // Assert
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        }
    }
}