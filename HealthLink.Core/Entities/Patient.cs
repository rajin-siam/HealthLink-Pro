using HealthLink.Core.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthLink.Core.Entities
{
    /// <summary>
    /// Represents a patient in the healthcare system.
    /// Stores personal, contact, and medical information, along with related records.
    /// </summary>
    public class Patient : BaseEntity
    {
        // ===========================
        // Basic Information
        // ===========================
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string BloodType { get; private set; }
        public decimal Height { get; private set; } // in cm
        public decimal Weight { get; private set; } // in kg

        // ===========================
        // Contact Information
        // ===========================
        public string? PhoneNumber { get; private set; }
        public string? Address { get; private set; }
        public string? City { get; private set; }

        // ===========================
        // Medical Essentials
        // ===========================
        public DateTime DateOfBirth { get; private set; }
        public string? Gender { get; private set; }

        // ===========================
        // Emergency Information
        // ===========================
        public string? EmergencyContactName { get; private set; }
        public string? EmergencyContactPhone { get; private set; }

        // ===========================
        // Insurance
        // ===========================
        public string? InsuranceProvider { get; private set; }
        public string? InsurancePolicyNumber { get; private set; }

        // ===========================
        // Computed Properties
        // ===========================
        public int Age
        {
            get
            {
                var today = DateTime.UtcNow;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        // ===========================
        // Navigation Properties
        // ===========================
        public virtual ICollection<Allergy> Allergies { get; private set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; private set; }
        public virtual ICollection<Appointment> Appointments { get; private set; }

        // ===========================
        // Constructors
        // ===========================
        protected Patient() : base()
        {
            Allergies = new List<Allergy>();
            MedicalRecords = new List<MedicalRecord>();
            Appointments = new List<Appointment>();
        }

        public Patient(
            Guid id,
            string name,
            string email,
            string bloodType,
            decimal height,
            decimal weight,
            DateTime dateOfBirth,
            string gender,
            string? phoneNumber,
            string? address,
            string? city,
            string? emergencyContactName,
            string? emergencyContactPhone,
            string? insuranceProvider,
            string? insurancePolicyNumber
        ) : base(id)
        {
            ValidateConstructorParameters(name, email, bloodType, height, weight, dateOfBirth);

            Name = name;
            Email = email;
            BloodType = bloodType;
            Height = height;
            Weight = weight;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Address = address;
            City = city;
            EmergencyContactName = emergencyContactName;
            EmergencyContactPhone = emergencyContactPhone;
            InsuranceProvider = insuranceProvider;
            InsurancePolicyNumber = insurancePolicyNumber;

            Allergies = new List<Allergy>();
            MedicalRecords = new List<MedicalRecord>();
            Appointments = new List<Appointment>();
        }

        // ===========================
        // Business Methods
        // ===========================
        public void UpdatePersonalInfo(string name, string email, decimal height, decimal weight)
        {
            ValidateConstructorParameters(name, email, BloodType, height, weight, DateOfBirth);

            Name = name;
            Email = email;
            Height = height;
            Weight = weight;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateContactInfo(string? phone, string? address, string? city)
        {
            PhoneNumber = phone;
            Address = address;
            City = city;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateEmergencyContact(string? contactName, string? contactPhone)
        {
            EmergencyContactName = contactName;
            EmergencyContactPhone = contactPhone;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateInsurance(string? provider, string? policyNumber)
        {
            InsuranceProvider = provider;
            InsurancePolicyNumber = policyNumber;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddAllergy(Allergy allergy)
        {
            if (allergy == null)
                throw new ArgumentNullException(nameof(allergy));

            if (Allergies.Any(a => a.Id == allergy.Id))
                throw new InvalidOperationException("Patient already has this allergy.");

            Allergies.Add(allergy);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveAllergy(Guid allergyId)
        {
            var allergy = Allergies.FirstOrDefault(a => a.Id == allergyId);
            if (allergy != null)
            {
                Allergies.Remove(allergy);
                UpdatedDate = DateTime.UtcNow;
            }
        }

        private void ValidateConstructorParameters(
            string name,
            string email,
            string bloodType,
            decimal height,
            decimal weight,
            DateTime dateOfBirth)
        {
            PatientValidator.ValidatePatientData(name, email, bloodType, height, weight);
            if (dateOfBirth > DateTime.UtcNow)
                throw new ArgumentException("Date of birth cannot be in the future.");
        }
    }
}
