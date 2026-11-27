using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("impact_exposure_dossiers")]
    public class ImpactExposureDossier
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("entity_id"), MaxLength(50)]
        public string EntityId { get; set; } = string.Empty;

        [Required, Column("entity_name"), MaxLength(150)]
        public string EntityName { get; set; } = string.Empty;

        [Required, Column("tier"), MaxLength(100)]
        public string Tier { get; set; } = string.Empty;

        [Required, Column("risk_level"), MaxLength(50)]
        public string RiskLevel { get; set; } = string.Empty;

        [Required, Column("location"), MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required, Column("po_reference"), MaxLength(50)]
        public string PoReference { get; set; } = string.Empty;

        [Column("value_at_risk", TypeName = "decimal(14,2)")]
        public decimal ValueAtRisk { get; set; }

        [Column("buffer_remaining_minutes")]
        public int BufferRemainingMinutes { get; set; }

        [Required, Column("sla_penalty_rate"), MaxLength(100)]
        public string SlaPenaltyRate { get; set; } = string.Empty;

        [Required, Column("clinical_or_business_impact")]
        public string ClinicalOrBusinessImpact { get; set; } = string.Empty;

        [Required, Column("assigned_carrier"), MaxLength(100)]
        public string AssignedCarrier { get; set; } = string.Empty;

        [Required, Column("cargo_details")]
        public string CargoDetails { get; set; } = string.Empty;

        public Incident? Incident { get; set; }
    }
}
