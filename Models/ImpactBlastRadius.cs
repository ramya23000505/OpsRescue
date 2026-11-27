using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("impact_blast_radiuses")]
    public class ImpactBlastRadius
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Column("root_incidents")]
        public int RootIncidents { get; set; } = 1;

        [Column("shipments")]
        public int Shipments { get; set; }

        [Column("purchase_orders")]
        public int PurchaseOrders { get; set; }

        [Column("consignees")]
        public int Consignees { get; set; }

        public Incident? Incident { get; set; }
    }
}
