using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsRescue.Models
{
    [Table("sla_shipment_contexts")]
    public class SlaShipmentContext
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("incident_id"), MaxLength(50)]
        public string IncidentId { get; set; } = string.Empty;

        [Required, Column("consignee"), MaxLength(150)]
        public string Consignee { get; set; } = string.Empty;

        [Required, Column("master_sla"), MaxLength(150)]
        public string MasterSla { get; set; } = string.Empty;

        [Required, Column("criticality"), MaxLength(150)]
        public string Criticality { get; set; } = string.Empty;

        [Required, Column("temperature_status"), MaxLength(100)]
        public string TemperatureStatus { get; set; } = string.Empty;

        [Required, Column("assigned_vehicle"), MaxLength(100)]
        public string AssignedVehicle { get; set; } = string.Empty;


    }
}
