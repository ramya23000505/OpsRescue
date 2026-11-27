using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("sla_risk_assessments")]
    public class SlaRiskAssessment
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("shipment_id"), MaxLength(50)]
        public string ShipmentId { get; set; } = string.Empty;

        [Required, Column("route"), MaxLength(150)]
        public string Route { get; set; } = string.Empty;

        [Required, Column("severity"), MaxLength(50)]
        public string Severity { get; set; } = string.Empty;

        [Column("breach_probability", TypeName = "decimal(5,2)")]
        public decimal BreachProbability { get; set; }

        [Required, Column("risk_tier"), MaxLength(50)]
        public string RiskTier { get; set; } = string.Empty;

        [Required, Column("risk_baseline_delta"), MaxLength(50)]
        public string RiskBaselineDelta { get; set; } = string.Empty;

        [Column("time_to_sla_breach_minutes")]
        public int TimeToSlaBreachMinutes { get; set; }

        [Required, Column("hard_breach_threshold_time"), MaxLength(50)]
        public string HardBreachThresholdTime { get; set; } = string.Empty;

        [Required, Column("projected_delay"), MaxLength(50)]
        public string ProjectedDelay { get; set; } = string.Empty;

        [Required, Column("committed_sla_window"), MaxLength(50)]
        public string CommittedSlaWindow { get; set; } = string.Empty;

        [Column("financial_exposure", TypeName = "decimal(12,2)")]
        public decimal FinancialExposure { get; set; }

        [Required, Column("financial_exposure_type"), MaxLength(100)]
        public string FinancialExposureType { get; set; } = "Contractual liquidated damages";

        [Required, Column("remaining_buffer_badge"), MaxLength(100)]
        public string RemainingBufferBadge { get; set; } = string.Empty;

        public Incident? Incident { get; set; }

        public List<SlaTrendPoint> TrendPoints { get; set; } = new();
        public List<SlaContributingFactor> ContributingFactors { get; set; } = new();
        public SlaShipmentContext? ShipmentContext { get; set; }
    }
}
