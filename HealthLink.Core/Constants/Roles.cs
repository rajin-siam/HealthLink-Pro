
namespace HealthLink.Core.Constants
{
    public static class Roles
    {
        public const string Patient = "Patient";
        public const string Doctor = "Doctor";
        public const string HospitalAdmin = "HospitalAdmin";
        public const string SystemAdmin = "SystemAdmin";

        public static bool IsValidRole(string role)
        {
            return GetAllRoles().Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public static IEnumerable<string> GetAllRoles()
        {
            return new List<string>
            {
                Patient,
                Doctor,
                HospitalAdmin,
                SystemAdmin
            };
        }
    }
}