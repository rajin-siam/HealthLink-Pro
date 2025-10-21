using System;
using HealthLink.Core.Enums;

namespace HealthLink.Core.Entities
{
    public class Doctor
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string LicenseNumber { get; private set; }
        public Specialization Specialization { get; private set; }
        public int YearsOfExperience { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }

        // Constructor
        public Doctor(Guid id, string name, string email, string licenseNumber,
                      Specialization specialization, int yearsOfExperience)
        {
            ValidateDoctorData(name, email, licenseNumber, yearsOfExperience);

            Id = id;
            Name = name;
            Email = email;
            LicenseNumber = licenseNumber;
            Specialization = specialization;
            YearsOfExperience = yearsOfExperience;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        // Parameterless constructor for Entity Framework
        protected Doctor() { }

        private void ValidateDoctorData(string name, string email, string licenseNumber, int yearsOfExperience)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Doctor name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("Email must be a valid email address.", nameof(email));

            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number cannot be empty.", nameof(licenseNumber));

            if (yearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(yearsOfExperience));
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}