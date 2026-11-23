using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, Column("firstname"), MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, Column("lastname"), MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, Column("username"), MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, Column("email"), MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Column("phone_number"), MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, Column("password")]
        public string Password { get; set; } = string.Empty;

        [Required, Column("role"), MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OperationsManagerProfile? OperationsManagerProfile { get; set; }
        public OperationsUserProfile? OperationsUserProfile { get; set; }
        public DeliveryPartnerProfile? DeliveryPartnerProfile { get; set; }
    }
}