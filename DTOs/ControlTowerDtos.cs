namespace OpsRescue.DTOs
{
    public class ControlTowerDashboardDto
    {
        public ControlTowerKpisDto Kpis { get; set; } = new();
        public List<ControlTowerIncidentItemDto> LiveDisruptions { get; set; } = new();
        public List<FleetReadinessDto> FleetReadiness { get; set; } = new();
        public SlaRiskDistributionDto SlaDistribution { get; set; } = new();
        public SystemStatusDto SystemStatus { get; set; } = new();
    }

    public class ControlTowerKpisDto
    {
        public int TotalActiveDisruptions { get; set; }
        public int CriticalP1Count { get; set; }
        public int HighP2Count { get; set; }
        public int ImminentSlaBreachesCount { get; set; }
        public decimal TotalRevenueAtRisk { get; set; }
        public string TotalRevenueAtRiskFormatted { get; set; } = string.Empty;
        public int PendingApprovalsCount { get; set; }
        public int ActiveRecoveriesCount { get; set; }
        public decimal AverageTrustScorePct { get; set; }
    }

    public class ControlTowerIncidentItemDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RemainingSlaBufferMinutes { get; set; }
        public string RemainingSlaBufferFormatted { get; set; } = string.Empty;
        public string LiquidatedPenaltyFormatted { get; set; } = string.Empty;
        public int TotalBlastRadius { get; set; }
        public int AffectedConsignees { get; set; }
        public string AssignedCarrier { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class FleetReadinessDto
    {
        public string PartnerName { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Standby Ready"; // Standby Ready, Dispatched, On Mission
        public string Specialization { get; set; } = "Cold-Chain Reefer";
        public string EtaToCorridor { get; set; } = "15 mins";
    }

    public class SlaRiskDistributionDto
    {
        public int CriticalBreachRisk { get; set; } // > 80% prob
        public int ElevatedRisk { get; set; }      // 50-80% prob
        public int ManagedRisk { get; set; }       // < 50% prob
    }

    public class SystemStatusDto
    {
        public string OperationalMode { get; set; } = "Active Governed Intelligence";
        public string SignalRHubStatus { get; set; } = "Connected (Broadcasting)";
        public string ActiveCausalEngine { get; set; } = "Online (v2.4)";
        public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;
    }
}
