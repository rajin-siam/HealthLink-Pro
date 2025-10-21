
using System.Net.Mail;

namespace HealthLink.Core.Validators
{
    public static class PatientValidator
    {
        public const decimal MinHeight = 50m;
        public const decimal MaxHeight = 300m;
        public const decimal MinWeight = 5m;
        public const decimal MaxWeight = 500m;

        public static void ValidatePatientData(string name, string email, string bloodType, decimal height, decimal weight)
        {
            ValidateName(name);
            ValidateEmail(email);
            ValidateBloodType(bloodType);
            ValidateHeight(height);
            ValidateWeight(weight);
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        private static void ValidateEmail(string email)
        {
            if (!IsValidEmail(email))
                throw new ArgumentException("Email must be a valid email address.", nameof(email));
        }

        private static void ValidateBloodType(string bloodType)
        {
            if (string.IsNullOrWhiteSpace(bloodType))
                throw new ArgumentException("Blood type cannot be empty.", nameof(bloodType));
        }

        private static void ValidateHeight(decimal height)
        {
            if (height < MinHeight || height > MaxHeight)
                throw new ArgumentException($"Height must be between {MinHeight}cm and {MaxHeight}cm.", nameof(height));
        }

        private static void ValidateWeight(decimal weight)
        {
            if (weight < MinWeight || weight > MaxWeight)
                throw new ArgumentException($"Weight must be between {MinWeight}kg and {MaxWeight}kg.", nameof(weight));
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}