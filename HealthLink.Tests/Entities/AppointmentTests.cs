
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
            var appointmentTime = DateTime.UtcNow.AddDays(7); // Next week
            var reasonForVisit = "Annual checkup";

            // Act
            var appointment = new Appointment(
                id: appointmentId,
                appointmentDateTime: appointmentTime,
                reasonForVisit: reasonForVisit
            );

            // Assert
            Assert.Equal(appointmentId, appointment.Id);
            Assert.Equal(appointmentTime, appointment.AppointmentDateTime);
            Assert.Equal(reasonForVisit, appointment.ReasonForVisit);
            Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
            Assert.Equal(30, appointment.DurationMinutes); // Default duration
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
                    appointmentDateTime: pastTime
                )
            );

            Assert.Contains("future", exception.Message.ToLower());
        }

        [Fact]
        public void CreateAppointment_WithNullReason_ShouldSucceed()
        {
            // Arrange
            var appointmentTime = DateTime.UtcNow.AddDays(7);

            // Act
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: appointmentTime,
                reasonForVisit: null
            );

            // Assert
            Assert.Null(appointment.ReasonForVisit);
            Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        }

        [Fact]
        public void Appointment_AssignPatient_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var patient = new Patient(
                id: Guid.NewGuid(),
                name: "John Doe",
                email: "john@example.com",
                bloodType: "O+",
                height: 180,
                weight: 75
            );

            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.AssignPatient(patient);

            // Assert
            Assert.Equal(patient, appointment.Patient);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_AssignNullPatient_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                appointment.AssignPatient(null)
            );
        }

        [Fact]
        public void Appointment_AssignDoctor_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var doctor = new Doctor(
                id: Guid.NewGuid(),
                name: "Dr. Smith",
                email: "smith@hospital.com",
                licenseNumber: "LIC123456",
                specialization: Specialization.Cardiology,
                yearsOfExperience: 10
            );

            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.AssignDoctor(doctor);

            // Assert
            Assert.Equal(doctor, appointment.Doctor);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_AssignNullDoctor_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                appointment.AssignDoctor(null)
            );
        }

        [Fact]
        public void Appointment_SetDuration_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var newDuration = 60;
            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.SetDuration(newDuration);

            // Assert
            Assert.Equal(newDuration, appointment.DurationMinutes);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_SetZeroDuration_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var zeroDuration = 0;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                appointment.SetDuration(zeroDuration)
            );

            Assert.Contains("greater than 0", exception.Message);
        }

        [Fact]
        public void Appointment_SetNegativeDuration_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var negativeDuration = -30;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                appointment.SetDuration(negativeDuration)
            );

            Assert.Contains("greater than 0", exception.Message);
        }

        [Fact]
        public void Appointment_Reschedule_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var newDateTime = DateTime.UtcNow.AddDays(14);
            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.Reschedule(newDateTime);

            // Assert
            Assert.Equal(newDateTime, appointment.AppointmentDateTime);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_RescheduleWithPastDate_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var pastDateTime = DateTime.UtcNow.AddDays(-1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                appointment.Reschedule(pastDateTime)
            );

            Assert.Contains("future", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_RescheduleCompletedAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.MarkAsCompleted("Consultation completed");

            var newDateTime = DateTime.UtcNow.AddDays(14);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.Reschedule(newDateTime)
            );

            Assert.Contains("completed", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_RescheduleCancelledAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.Cancel("Patient cancelled");

            var newDateTime = DateTime.UtcNow.AddDays(14);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.Reschedule(newDateTime)
            );

            Assert.Contains("cancelled", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_MarkAsCompleted_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var notes = "Consultation completed successfully";
            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.MarkAsCompleted(notes);

            // Assert
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);
            Assert.Equal(notes, appointment.Notes);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_MarkAsCompletedWithNullNotes_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Act
            appointment.MarkAsCompleted(null);

            // Assert
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);
            Assert.Null(appointment.Notes);
        }

        [Fact]
        public void Appointment_MarkAsCompletedOnCancelledAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.Cancel("Doctor emergency");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.MarkAsCompleted("Notes")
            );

            Assert.Contains("scheduled", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_MarkAsCompletedOnNoShowAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.MarkAsNoShow();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.MarkAsCompleted("Notes")
            );

            Assert.Contains("scheduled", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_Cancel_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var reason = "Doctor emergency";
            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.Cancel(reason);

            // Assert
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.Equal(reason, appointment.Notes);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_CancelWithNullReason_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Act
            appointment.Cancel(null);

            // Assert
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.Null(appointment.Notes);
        }

        [Fact]
        public void Appointment_CancelCompletedAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.MarkAsCompleted("Consultation done");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.Cancel("Reason")
            );

            Assert.Contains("completed", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_CancelAlreadyCancelledAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.Cancel("First cancellation");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.Cancel("Second cancellation")
            );

            Assert.Contains("cancelled", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_MarkAsNoShow_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            var originalUpdatedDate = appointment.UpdatedDate;

            // Act
            appointment.MarkAsNoShow();

            // Assert
            Assert.Equal(AppointmentStatus.NoShow, appointment.Status);
            Assert.True(appointment.UpdatedDate > originalUpdatedDate);
        }

        [Fact]
        public void Appointment_MarkAsNoShowOnCompletedAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.MarkAsCompleted("Consultation done");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.MarkAsNoShow()
            );

            Assert.Contains("scheduled", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_MarkAsNoShowOnCancelledAppointment_ShouldThrow()
        {
            // Arrange
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            appointment.Cancel("Patient cancelled");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                appointment.MarkAsNoShow()
            );

            Assert.Contains("scheduled", exception.Message.ToLower());
        }

        [Fact]
        public void Appointment_StatusTransitions_ShouldBeValid()
        {
            // Test valid state transitions
            var appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Scheduled -> Completed
            appointment.MarkAsCompleted("Done");
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);

            // Reset for next test
            appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Scheduled -> Cancelled
            appointment.Cancel("Cancelled");
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);

            // Reset for next test
            appointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentDateTime: DateTime.UtcNow.AddDays(7)
            );

            // Scheduled -> NoShow
            appointment.MarkAsNoShow();
            Assert.Equal(AppointmentStatus.NoShow, appointment.Status);
        }
    }
}