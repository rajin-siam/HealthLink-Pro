using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthLink.Core.Entities
{
    public class Payment : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public virtual Appointment Appointment { get; set; }

        public decimal Amount { get; private set; }              // ✅ $150.00
        public PaymentMethod Method { get; private set; }        // ✅ Credit Card
        public PaymentStatus Status { get; private set; }        // ✅ Completed
        public string TransactionId { get; private set; }        // ✅ "TXN-12345"
        public DateTime? PaidAt { get; private set; }            // ✅ Jan 15, 2025 2:30 PM
        public string Notes { get; private set; }                // ✅ "Insurance covered 80%"
    }

    public enum PaymentMethod
    {
        Cash = 1,
        CreditCard = 2,
        DebitCard = 3,
        Insurance = 4,
        OnlinePayment = 5
    }

    public enum PaymentStatus
    {
        Pending = 1,      // ✅ Invoice sent, not paid
        Completed = 2,    // ✅ Payment received
        Failed = 3,       // ✅ Credit card declined
        Refunded = 4      // ✅ Money returned
    }
}
