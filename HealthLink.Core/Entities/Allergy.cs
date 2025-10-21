using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{

    /// Represents an allergy that a patient has.
    /// Tracks allergy information including severity and reaction details.
    public class Allergy : BaseEntity
    {
        // Allergy Information
        public string Name { get; private set; }
        public AllergySeverity Severity { get; private set; }
        public string ReactionDescription { get; private set; }
        public DateTime IdentifiedDate { get; private set; }

        // Navigation Properties
        // FK property: PatientId will be created automatically by EF Core
        public virtual Patient Patient { get; set; } // Many-to-One: Allergy belongs to one Patient

        // Constructor
        public Allergy(Guid id, string name, AllergySeverity severity,
            string reactionDescription = null, DateTime? identifiedDate = null)
            : base(id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Allergy name cannot be empty.", nameof(name));

            Name = name;
            Severity = severity;
            ReactionDescription = reactionDescription;
            IdentifiedDate = identifiedDate ?? DateTime.UtcNow;
        }

        // Parameterless constructor for EF Core
        protected Allergy() : base() { }

        // Business Methods
        public void AssignToPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            Patient = patient;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateSeverity(AllergySeverity newSeverity)
        {
            Severity = newSeverity;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateReactionDescription(string description)
        {
            ReactionDescription = description;
            UpdatedDate = DateTime.UtcNow;
        }

        public void SetIdentifiedDate(DateTime date)
        {
            if (date > DateTime.UtcNow)
                throw new ArgumentException("Identified date cannot be in the future.", nameof(date));

            IdentifiedDate = date;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}