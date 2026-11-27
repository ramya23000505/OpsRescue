using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("accountability_trust_scores")]
    public class AccountabilityTrustScore
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("recovery_strategy_id"), MaxLength(50)]
        public string RecoveryStrategyId { get; set; } = string.Empty;

        [Column("predicted_recovery_time_mins")]
        public int PredictedRecoveryTimeMins { get; set; }

        [Column("actual_recovery_time_mins")]
        public int ActualRecoveryTimeMins { get; set; }

        [Column("trust_score_pct", TypeName = "decimal(5,2)")]
        public decimal TrustScorePct { get; set; }

        [Required, Column("prediction_accuracy_grade"), MaxLength(50)]
        public string PredictionAccuracyGrade { get; set; } = string.Empty;

        [Required, Column("variance_reason")]
        public string VarianceReason { get; set; } = string.Empty;

        [Column("evaluated_at")]
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
