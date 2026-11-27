namespace OpsRescue.DTOs
{
    public class TrustScoreDetailDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string RecoveryStrategyId { get; set; } = string.Empty;
        public string StrategyTitle { get; set; } = string.Empty;
        public int PredictedRecoveryTimeMins { get; set; }
        public int ActualRecoveryTimeMins { get; set; }
        public int VarianceMinutes { get; set; }
        public decimal TrustScorePct { get; set; }
        public string PredictionAccuracyGrade { get; set; } = string.Empty;
        public string VarianceReason { get; set; } = string.Empty;
        public DateTime EvaluatedAt { get; set; }
        public string EvaluatedAtFormatted { get; set; } = string.Empty;
    }

    public class TrustScoreSummaryDto
    {
        public decimal OverallPlatformTrustScorePct { get; set; }
        public string OverallAccuracyGrade { get; set; } = "Grade A (96.4%)";
        public int TotalEvaluatedIncidents { get; set; }
        public decimal AverageVarianceMinutes { get; set; }
        public List<TrustScoreDetailDto> RecentEvaluations { get; set; } = new();
    }
}
