using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    /// Represents an appointment between a patient and a doctor.
    /// Tracks appointment status and maintains relationship history.
    public class Appointment : BaseEntity
    {
        // Appointment Details
        public DateTime AppointmentDateTime { get; private set; }
        public int DurationMinutes { get; private set; } = 30;
        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
        public string Notes { get; private set; }
        public string ReasonForVisit { get; private set; }

        // Navigation Properties
        public virtual Patient Patient { get; set; } // Many-to-One: Appointment belongs to one Patient
        public virtual Doctor Doctor { get; set; }   // Many-to-One: Appointment belongs to one Doctor

        // Constructor
        public Appointment(Guid id, DateTime appointmentDateTime, string reasonForVisit = null)
            : base(id)
        {
            if (appointmentDateTime < DateTime.UtcNow)
                throw new ArgumentException("Appointment must be scheduled for a future date.", nameof(appointmentDateTime));

            AppointmentDateTime = appointmentDateTime;
            ReasonForVisit = reasonForVisit;
        }

        // Parameterless constructor for EF Core
        protected Appointment() : base() { }

        // Business Methods
        public void AssignPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            Patient = patient;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AssignDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            Doctor = doctor;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetDuration(int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentException("Duration must be greater than 0.", nameof(minutes));

            DurationMinutes = minutes;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Reschedule(DateTime newDateTime)
        {
            if (newDateTime < DateTime.UtcNow)
                throw new ArgumentException("New appointment time must be in the future.", nameof(newDateTime));

            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot reschedule a completed or cancelled appointment.");

            AppointmentDateTime = newDateTime;
            UpdatedDate = DateTime.UtcNow;
        }

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

        public void MarkAsNoShow()
        {
            if (Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Only scheduled appointments can be marked as no-show.");

            Status = AppointmentStatus.NoShow;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}