using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("simulation_strategies")]
    public class SimulationStrategy
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("strategy_id"), MaxLength(50)]
        public string StrategyId { get; set; } = string.Empty;

        [Column("strategy_number")]
        public int StrategyNumber { get; set; }

        [Required, Column("title"), MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, Column("category_tag"), MaxLength(50)]
        public string CategoryTag { get; set; } = string.Empty;

        [Required, Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("is_recommended")]
        public bool IsRecommended { get; set; }

        [Column("recommendation_badge"), MaxLength(100)]
        public string? RecommendationBadge { get; set; }

        [Required, Column("predicted_delivery_time"), MaxLength(50)]
        public string PredictedDeliveryTime { get; set; } = string.Empty;

        [Required, Column("delivery_margin"), MaxLength(50)]
        public string DeliveryMargin { get; set; } = string.Empty;

        [Column("estimated_outlay", TypeName = "decimal(12,2)")]
        public decimal EstimatedOutlay { get; set; }

        [Required, Column("outlay_breakdown"), MaxLength(100)]
        public string OutlayBreakdown { get; set; } = string.Empty;

        [Required, Column("expected_sla_outcome"), MaxLength(100)]
        public string ExpectedSlaOutcome { get; set; } = string.Empty;

        [Column("sla_success_probability", TypeName = "decimal(5,2)")]
        public decimal SlaSuccessProbability { get; set; }

        [Column("liquidated_damages_impact", TypeName = "decimal(12,2)")]
        public decimal LiquidatedDamagesImpact { get; set; }

        [Column("is_damages_avoided")]
        public bool IsDamagesAvoided { get; set; }

        [Required, Column("operational_protocol")]
        public string OperationalProtocol { get; set; } = string.Empty;

        [Column("identified_constraints")]
        public string? IdentifiedConstraints { get; set; }

        [Column("operational_governance")]
        public string? OperationalGovernance { get; set; }

        [Required, Column("required_approval_tier"), MaxLength(50)]
        public string RequiredApprovalTier { get; set; } = "Tier 1";

        [Column("governance_alert")]
        public string? GovernanceAlert { get; set; }


    }
}
