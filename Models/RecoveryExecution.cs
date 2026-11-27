using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("recovery_executions")]
    public class RecoveryExecution
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("execution_id"), MaxLength(50)]
        public string ExecutionId { get; set; } = string.Empty;

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("strategy_id"), MaxLength(50)]
        public string StrategyId { get; set; } = string.Empty;

        [Required, Column("strategy_name"), MaxLength(200)]
        public string StrategyName { get; set; } = string.Empty;

        [Column("dispatched_at")]
        public DateTime DispatchedAt { get; set; } = DateTime.UtcNow;

        [Column("estimated_rendezvous_at")]
        public DateTime? EstimatedRendezvousAt { get; set; }

        [Column("actual_rendezvous_at")]
        public DateTime? ActualRendezvousAt { get; set; }

        [Column("resolved_at")]
        public DateTime? ResolvedAt { get; set; }

        [Required, Column("status"), MaxLength(50)]
        public string Status { get; set; } = "Dispatched";

        [Required, Column("assigned_partner"), MaxLength(150)]
        public string AssignedPartner { get; set; } = string.Empty;

        [Required, Column("vehicle_id"), MaxLength(100)]
        public string VehicleId { get; set; } = string.Empty;

        [Required, Column("current_location"), MaxLength(150)]
        public string CurrentLocation { get; set; } = string.Empty;

        [Column("action_notes")]
        public string? ActionNotes { get; set; }
    }
}
