namespace OpsRescue.DTOs
{
    public class RecoverySimulationResponseDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string ShipmentId { get; set; } = string.Empty;
        public string OptimizationEngineHeadline { get; set; } = string.Empty;
        public string RecommendedStrategyId { get; set; } = string.Empty;
        public string RecommendationSummary { get; set; } = string.Empty;

        // The 3 Comparative Strategies (Slide 7)
        public List<RecoveryStrategyDto> Strategies { get; set; } = new();

        // Trade-Off Matrix for Decision Making
        public StrategyTradeoffSummaryDto TradeoffSummary { get; set; } = new();
    }

    public class RecoveryStrategyDto
    {
        public string StrategyId { get; set; } = string.Empty; // "STRAT-01", "STRAT-02", "STRAT-03"
        public int StrategyNumber { get; set; } // 1, 2, 3
        public string Title { get; set; } = string.Empty; // "Alternate Route Bypass", "Relief Cross-Dock Transfer", "Secondary Depot Swap"
        public string CategoryTag { get; set; } = string.Empty; // "Continuation", "Optimized", "Stock Swap"
        public string Description { get; set; } = string.Empty;
        public bool IsRecommended { get; set; }
        public string? RecommendationBadge { get; set; } // "RECOMMENDED STRATEGY · Zero SLA Breach"

        // Metrics
        public string PredictedDeliveryTime { get; set; } = string.Empty; // "11:24 AM IST"
        public string DeliveryMargin { get; set; } = string.Empty; // "-6m Prior to Cut-off", "+12m Breach"
        public decimal EstimatedOutlay { get; set; } // 34500
        public string EstimatedOutlayFormatted { get; set; } = string.Empty; // "₹ 34,500"
        public string OutlayBreakdown { get; set; } = string.Empty; // "Reefer + Transshipment"

        public string ExpectedSlaOutcome { get; set; } = string.Empty; // "100% On-Time Preserved", "Breach / Partial Penalty"
        public double SlaSuccessProbability { get; set; } // 97.8
        public string SlaSuccessProbabilityFormatted { get; set; } = string.Empty; // "97.8%"

        public decimal LiquidatedDamagesImpact { get; set; }
        public string LiquidatedDamagesImpactFormatted { get; set; } = string.Empty; // "₹ 1,35,000 Avoided" or "₹ 45,000 Penalty"
        public bool IsDamagesAvoided { get; set; } // true if savings/avoided, false if penalty

        // Operational Execution Details
        public string OperationalProtocol { get; set; } = string.Empty;
        public string? IdentifiedConstraints { get; set; }
        public string? OperationalGovernance { get; set; }
        public string RequiredApprovalTier { get; set; } = string.Empty; // "Tier 1", "Tier 2", "Tier 4"
        public string? GovernanceAlert { get; set; }
    }

    public class StrategyTradeoffSummaryDto
    {
        public string KeyTradeoffFinding { get; set; } = string.Empty;
        public string NetBenefitFormatted { get; set; } = string.Empty;
        public string WhyRecommended { get; set; } = string.Empty;
        public string WhyRejectedAlternatives { get; set; } = string.Empty;
    }
}
