using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("incident_timeline_events")]
    public class IncidentTimelineEvent
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Column("event_time")]
        public DateTime EventTime { get; set; }

        [Required, Column("stage"), MaxLength(100)]
        public string Stage { get; set; } = string.Empty;

        [Required, Column("title"), MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, Column("description")]
        public string Description { get; set; } = string.Empty;

        [Required, Column("severity_badge"), MaxLength(50)]
        public string SeverityBadge { get; set; } = string.Empty;

        [Required, Column("source"), MaxLength(100)]
        public string Source { get; set; } = "IoT Telematics";
    }
}
