using System;
using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    public class Appointment
    {
        public Guid Id { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public DateTime AppointmentDateTime { get; private set; }
        public int DurationMinutes { get; private set; } = 30;
        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
        public string Notes { get; private set; }
        public string ReasonForVisit { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }

        public Appointment(Guid id, Guid patientId, Guid doctorId, DateTime appointmentDateTime)
        {
            if (appointmentDateTime < DateTime.UtcNow)
                throw new ArgumentException("Appointment must be scheduled for a future date.", nameof(appointmentDateTime));

            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            AppointmentDateTime = appointmentDateTime;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        protected Appointment() { }

        public void MarkAsCompleted(string notes)
        {
            if (Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Only scheduled appointments can be completed.");

            Status = AppointmentStatus.Completed;
            Notes = notes;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel a completed or already cancelled appointment.");

            Status = AppointmentStatus.Cancelled;
            Notes = reason;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}