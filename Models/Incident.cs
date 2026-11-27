using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("incidents")]
    public class Incident
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("incident_type"), MaxLength(150)]
        public string IncidentType { get; set; } = string.Empty;

        [Required, Column("root_cause")]
        public string RootCause { get; set; } = string.Empty;

        [Required, Column("severity"), MaxLength(50)]
        public string Severity { get; set; } = string.Empty;

        [Required, Column("propagation_model"), MaxLength(100)]
        public string PropagationModel { get; set; } = "Active Causal Graph";

        [Required, Column("status"), MaxLength(50)]
        public string Status { get; set; } = "Synced";

        [Column("cargo_value", TypeName = "decimal(14,2)")]
        public decimal CargoValue { get; set; }

        [Column("liquidated_penalty", TypeName = "decimal(14,2)")]
        public decimal LiquidatedPenalty { get; set; }

        [Column("remaining_sla_buffer_minutes")]
        public int RemainingSlaBufferMinutes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ImpactBlastRadius? BlastRadius { get; set; }
        public List<ImpactExposureDossier> ExposureDossiers { get; set; } = new();
        public List<ImpactGraphNode> GraphNodes { get; set; } = new();
        public List<ImpactGraphEdge> GraphEdges { get; set; } = new();
        public IncidentSimulation? Simulation { get; set; }
        public SlaRiskAssessment? SlaRiskAssessment { get; set; }
        public List<IncidentTimelineEvent> TimelineEvents { get; set; } = new();
        public List<RecoveryExecution> RecoveryExecutions { get; set; } = new();
        public List<ManagerApprovalLog> ApprovalLogs { get; set; } = new();
        public List<AccountabilityTrustScore> TrustScores { get; set; } = new();
    }
}
