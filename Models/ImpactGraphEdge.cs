using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("impact_graph_edges")]
    public class ImpactGraphEdge
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("from_node"), MaxLength(50)]
        public string FromNode { get; set; } = string.Empty;

        [Required, Column("to_node"), MaxLength(50)]
        public string ToNode { get; set; } = string.Empty;

        [Required, Column("relationship"), MaxLength(50)]
        public string Relationship { get; set; } = string.Empty;

        [Column("label"), MaxLength(100)]
        public string? Label { get; set; }

        [Required, Column("severity"), MaxLength(50)]
        public string Severity { get; set; } = "High";

        public Incident? Incident { get; set; }
    }
}
