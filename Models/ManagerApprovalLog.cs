using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("manager_approval_logs")]
    public class ManagerApprovalLog
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("strategy_id"), MaxLength(50)]
        public string StrategyId { get; set; } = string.Empty;

        [Required, Column("strategy_name"), MaxLength(200)]
        public string StrategyName { get; set; } = string.Empty;

        [Required, Column("approved_by"), MaxLength(100)]
        public string ApprovedBy { get; set; } = string.Empty;

        [Column("approved_at")]
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;

        [Required, Column("approval_tier"), MaxLength(50)]
        public string ApprovalTier { get; set; } = "Tier 1";

        [Column("notes")]
        public string? Notes { get; set; }
    }
}
