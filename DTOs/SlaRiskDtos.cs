namespace OpsRescue.DTOs
{
    public class SlaRiskAnalysisResponseDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string ShipmentId { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;

        // Top 4 Metrics (Slide 6)
        public double BreachProbability { get; set; } // e.g. 94.2
        public string BreachProbabilityFormatted { get; set; } = string.Empty; // "94.2%"
        public string RiskTier { get; set; } = "Critical Risk"; // "Critical Risk", "High Risk", "Moderate Risk"
        public string RiskBaselineDelta { get; set; } = "+18.4% vs baseline";

        public int TimeToSlaBreachMinutes { get; set; } // 38
        public string TimeToSlaBreachFormatted { get; set; } = string.Empty; // "38 min"
        public string HardBreachThresholdTime { get; set; } = string.Empty; // "11:30 AM IST"

        public string ProjectedDelay { get; set; } = string.Empty; // "+1h 48m"
        public string CommittedSlaWindow { get; set; } = string.Empty; // "11:30 AM IST"

        public decimal FinancialExposure { get; set; } // 135000
        public string FinancialExposureFormatted { get; set; } = string.Empty; // "₹ 1.35L"
        public string FinancialExposureType { get; set; } = "Contractual liquidated damages";

        // Remaining Buffer Badge
        public string RemainingBufferBadge { get; set; } = "Remaining Buffer: 38 min";

        // Timeline Trend Points for Chart (Progression over transit timeline)
        public List<SlaTrendPointDto> BreachProbabilityTrend { get; set; } = new();

        // Top Contributing Risk Factors (58%, 24%, 18%)
        public List<RiskContributingFactorDto> ContributingFactors { get; set; } = new();

        // Selected Shipment Context
        public ShipmentContextDto ShipmentContext { get; set; } = new();
    }

    public class SlaTrendPointDto
    {
        public string TimeLabel { get; set; } = string.Empty; // "09:30", "10:00", "10:15", "10:30 (NOW)", "11:00", "11:30"
        public double NormalTransitProbability { get; set; }
        public double? BreachCurveProbability { get; set; }
        public bool IsCurrentPoint { get; set; }
        public bool IsHardBreachThreshold { get; set; }
    }

    public class RiskContributingFactorDto
    {
        public string FactorName { get; set; } = string.Empty; // e.g. "Vehicle Immobilization"
        public string SeverityBadge { get; set; } = string.Empty; // "Critical · 58% Contributor"
        public int ContributionPercentage { get; set; } // 58
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> KeyMetrics { get; set; } = new();
    }

    public class ShipmentContextDto
    {
        public string Consignee { get; set; } = string.Empty; // "Apollo Hospitals & Research"
        public string MasterSla { get; set; } = string.Empty; // "Tier 1 Cold-Chain"
        public string Criticality { get; set; } = string.Empty; // "High-priority Medical Supplies"
        public string TemperatureStatus { get; set; } = string.Empty; // "Safe (+4.1°C)"
        public string AssignedVehicle { get; set; } = string.Empty; // "TRK-882 (Reefer Unit)"
    }
}
