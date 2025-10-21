using System;

namespace HealthLink.Core.Entities
{
    public class Hospital
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string RegistrationNumber { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public Hospital(Guid id, string name, string registrationNumber, string address,
                       string city, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hospital name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(registrationNumber))
                throw new ArgumentException("Registration number cannot be empty.", nameof(registrationNumber));

            Id = id;
            Name = name;
            RegistrationNumber = registrationNumber;
            Address = address;
            City = city;
            PhoneNumber = phoneNumber;
            CreatedDate = DateTime.UtcNow;
        }

        protected Hospital() { }
    }
}