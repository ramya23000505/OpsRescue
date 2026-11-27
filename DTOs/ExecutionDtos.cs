using System.ComponentModel.DataAnnotations;

namespace OpsRescue.DTOs
{
    public class DispatchExecutionRequestDto
    {
        [Required]
        public string IncidentId { get; set; } = string.Empty;

        [Required]
        public string StrategyId { get; set; } = string.Empty;

        public string? AssignedPartner { get; set; }

        public string? VehicleId { get; set; }

        public string? ActionNotes { get; set; }
    }

    public class DispatchExecutionResponseDto
    {
        public string ExecutionId { get; set; } = string.Empty;
        public string IncidentId { get; set; } = string.Empty;
        public string StrategyId { get; set; } = string.Empty;
        public string StrategyName { get; set; } = string.Empty;
        public string Status { get; set; } = "Dispatched";
        public DateTime DispatchedAt { get; set; }
        public string DispatchedAtFormatted { get; set; } = string.Empty;
        public DateTime EstimatedRendezvousAt { get; set; }
        public string EstimatedRendezvousFormatted { get; set; } = string.Empty;
        public string AssignedPartner { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public string ApprovalTier { get; set; } = "Tier 1 Auto-Approved";
        public string Message { get; set; } = string.Empty;
    }
}
