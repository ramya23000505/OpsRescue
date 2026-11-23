using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("operations_manager_profiles")]
    public class OperationsManagerProfile
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("user_id")]
        public int UserId { get; set; }

        [Required, Column("employee_id"), MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required, Column("department"), MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required, Column("branch_location"), MaxLength(100)]
        public string BranchLocation { get; set; } = string.Empty;

        [Required, Column("designation"), MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}