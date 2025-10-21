using System;
using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    public class Allergy : BaseEntity
    {
        public Guid PatientId { get; private set; }  // ADD THIS
        public string Name { get; private set; }
        public AllergySeverity Severity { get; private set; }

        // Constructor
        public Allergy(Guid id, string name, AllergySeverity severity)
            : base(id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Allergy name cannot be empty.", nameof(name));

            Name = name;
            Severity = severity;
        }

        // Parameterless constructor for Entity Framework
        protected Allergy() : base() { }
    }
}