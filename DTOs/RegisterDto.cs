using System.ComponentModel.DataAnnotations;

namespace OpsRescue.DTOs
{
    public class RegisterDto
    {
        // Common user fields
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|Operations Manager|Operations User|Delivery Partner)$",
            ErrorMessage = "Role must be Admin, Operations Manager, Operations User, or Delivery Partner.")]
        public string Role { get; set; } = string.Empty;

        // Operations Manager & Operations User fields
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }
        public string? BranchLocation { get; set; }
        public string? Designation { get; set; }

        // Delivery Partner fields
        public string? PartnerId { get; set; }
        public string? CompanyName { get; set; }
        public string? VehicleNumber { get; set; }
        public string? OperatingLocation { get; set; }
    }
}