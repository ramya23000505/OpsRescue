using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("sla_trend_points")]
    public class SlaTrendPoint
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Required, Column("time_label"), MaxLength(50)]
        public string TimeLabel { get; set; } = string.Empty;

        [Column("normal_transit_probability", TypeName = "decimal(5,2)")]
        public decimal NormalTransitProbability { get; set; }

        [Column("breach_curve_probability", TypeName = "decimal(5,2)")]
        public decimal BreachCurveProbability { get; set; }

        [Column("is_current_point")]
        public bool IsCurrentPoint { get; set; }

        [Column("is_hard_breach_threshold")]
        public bool IsHardBreachThreshold { get; set; }


    }
}
