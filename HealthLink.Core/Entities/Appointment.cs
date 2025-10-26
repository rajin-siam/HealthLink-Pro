using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents an appointment between a patient and a doctor.
    /// Tracks appointment status, timing, payments, and accountability.
    /// </summary>
    public class Appointment : BaseEntity
    {
        // =========================
        // Appointment Details
        // =========================
        public DateTime ScheduledDateTime { get; private set; }
        public int DurationMinutes { get; private set; } = 30;
        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
        public string ReasonForVisit { get; private set; }
        public string Notes { get; private set; }

        // =========================
        // Tracking & Accountability
        // =========================
        public DateTime? ActualStartTime { get; private set; }   // When it actually started
        public DateTime? ActualEndTime { get; private set; }     // When it actually ended
        public string CancelledBy { get; private set; }          // “Doctor”, “Patient”, “System”
        public string CancelReason { get; private set; }         // Why it was cancelled

        // =========================
        // Payment Information
        // =========================
        public bool IsPaid { get; private set; } = false;        // Has payment been made
        public decimal? AmountPaid { get; private set; }         // Amount actually paid
        public DateTime? PaymentDate { get; private set; }       // When payment occurred
        public string PaymentMethod { get; private set; }        // “Cash”, “Card”, “Insurance”

        // =========================
        // Relationships
        // =========================
        public virtual Patient Patient { get; private set; }
        public virtual Doctor Doctor { get; private set; }

        // =========================
        // Constructors
        // =========================
        public Appointment(Guid id, DateTime scheduledDateTime, string reasonForVisit = null)
            : base(id)
        {
            if (scheduledDateTime < DateTime.UtcNow)
                throw new ArgumentException("Appointment must be scheduled for a future date.", nameof(scheduledDateTime));

            ScheduledDateTime = scheduledDateTime;
            ReasonForVisit = reasonForVisit;
        }

        protected Appointment() : base() { }

        // =========================
        // Business Methods
        // =========================

        public void AssignPatient(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            UpdatedDate = DateTime.UtcNow;
        }

        public void AssignDoctor(Doctor doctor)
        {
            Doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
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

            ScheduledDateTime = newDateTime;
            UpdatedDate = DateTime.UtcNow;
        }

        public void StartAppointment()
        {
            if (Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Only scheduled appointments can be started.");

            ActualStartTime = DateTime.UtcNow;
            Status = AppointmentStatus.InProgress;
            UpdatedDate = DateTime.UtcNow;
        }

        public void CompleteAppointment(string notes)
        {
            if (Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException("Only in-progress appointments can be completed.");

            ActualEndTime = DateTime.UtcNow;
            Status = AppointmentStatus.Completed;
            Notes = notes;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsPaid(decimal amount, string method)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be positive.", nameof(amount));

            IsPaid = true;
            AmountPaid = amount;
            PaymentMethod = method;
            PaymentDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Cancel(string cancelledBy, string reason)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel a completed or already cancelled appointment.");

            CancelledBy = cancelledBy;
            CancelReason = reason;
            Status = AppointmentStatus.Cancelled;
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
