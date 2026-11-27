using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("incident_simulations")]
    public class IncidentSimulation
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("shipment_id"), MaxLength(50)]
        public string ShipmentId { get; set; } = string.Empty;

        [Required, Column("recommended_strategy_id"), MaxLength(50)]
        public string RecommendedStrategyId { get; set; } = string.Empty;

        [Required, Column("recommendation_summary")]
        public string RecommendationSummary { get; set; } = string.Empty;

        [Required, Column("key_tradeoff_finding")]
        public string KeyTradeoffFinding { get; set; } = string.Empty;

        [Required, Column("why_recommended")]
        public string WhyRecommended { get; set; } = string.Empty;

        [Required, Column("why_rejected_alternatives")]
        public string WhyRejectedAlternatives { get; set; } = string.Empty;

        public Incident? Incident { get; set; }

        public List<SimulationStrategy> Strategies { get; set; } = new();
    }
}
