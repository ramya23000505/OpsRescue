using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("delivery_partner_profiles")]
    public class DeliveryPartnerProfile
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("user_id")]
        public int UserId { get; set; }

        [Required, Column("partner_id"), MaxLength(50)]
        public string PartnerId { get; set; } = string.Empty;

        [Required, Column("company_name"), MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Required, Column("vehicle_number"), MaxLength(50)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required, Column("operating_location"), MaxLength(100)]
        public string OperatingLocation { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}