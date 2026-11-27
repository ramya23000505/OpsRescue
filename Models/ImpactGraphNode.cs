using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("impact_graph_nodes")]
    public class ImpactGraphNode
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("node_id"), MaxLength(50)]
        public string NodeId { get; set; } = string.Empty;

        [Required, Column("label"), MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [Column("tier_level")]
        public int TierLevel { get; set; }

        [Required, Column("node_type"), MaxLength(50)]
        public string NodeType { get; set; } = string.Empty;

        [Required, Column("status"), MaxLength(100)]
        public string Status { get; set; } = string.Empty;

        [Column("subtitle"), MaxLength(150)]
        public string Subtitle { get; set; } = string.Empty;

        [Column("attributes_json")]
        public string? AttributesJson { get; set; }

        public Incident? Incident { get; set; }
    }
}
