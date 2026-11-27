namespace OpsRescue.DTOs
{
    public class IncidentListDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
        public decimal CargoValue { get; set; }
        public string CargoValueFormatted { get; set; } = string.Empty;
        public decimal LiquidatedPenalty { get; set; }
        public string LiquidatedPenaltyFormatted { get; set; } = string.Empty;
        public int RemainingSlaBufferMinutes { get; set; }
        public string RemainingSlaBufferFormatted { get; set; } = string.Empty;
        public int TotalBlastRadius { get; set; }
        public int AffectedAccounts { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class IncidentTimelineEventDto
    {
        public int Id { get; set; }
        public DateTime EventTime { get; set; }
        public string EventTimeFormatted { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SeverityBadge { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }

    public class IncidentDetailDto : IncidentListDto
    {
        public string PropagationModel { get; set; } = "Active Causal Graph";
        public BlastRadiusBreakdownDto BlastRadiusBreakdown { get; set; } = new();
        public List<IncidentTimelineEventDto> Timeline { get; set; } = new();
        public RecoveryExecutionSummaryDto? LatestExecution { get; set; }
        public ManagerApprovalSummaryDto? LatestApproval { get; set; }
        public AccountabilityTrustScoreSummaryDto? TrustScore { get; set; }
    }

    public class RecoveryExecutionSummaryDto
    {
        public string ExecutionId { get; set; } = string.Empty;
        public string StrategyId { get; set; } = string.Empty;
        public string StrategyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DispatchedAtFormatted { get; set; } = string.Empty;
        public string EstimatedRendezvousFormatted { get; set; } = string.Empty;
        public string AssignedPartner { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
    }

    public class ManagerApprovalSummaryDto
    {
        public string StrategyId { get; set; } = string.Empty;
        public string StrategyName { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public string ApprovedAtFormatted { get; set; } = string.Empty;
        public string ApprovalTier { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class AccountabilityTrustScoreSummaryDto
    {
        public string RecoveryStrategyId { get; set; } = string.Empty;
        public int PredictedRecoveryTimeMins { get; set; }
        public int ActualRecoveryTimeMins { get; set; }
        public decimal TrustScorePct { get; set; }
        public string PredictionAccuracyGrade { get; set; } = string.Empty;
        public string VarianceReason { get; set; } = string.Empty;
    }
}
