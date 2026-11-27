using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("sla_contributing_factors")]
    public class SlaContributingFactor
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Required, Column("factor_name"), MaxLength(100)]
        public string FactorName { get; set; } = string.Empty;

        [Required, Column("severity_badge"), MaxLength(100)]
        public string SeverityBadge { get; set; } = string.Empty;

        [Column("contribution_percentage")]
        public int ContributionPercentage { get; set; }

        [Required, Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("key_metrics_json")]
        public string? KeyMetricsJson { get; set; }


    }
}
