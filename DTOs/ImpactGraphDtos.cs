namespace OpsRescue.DTOs
{
    public class ImpactAnalysisResponseDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string PropagationModel { get; set; } = "Active Causal Graph";
        public string Status { get; set; } = "Synced";

        // Top KPIs
        public int TotalBlastRadius { get; set; }
        public BlastRadiusBreakdownDto BlastRadiusBreakdown { get; set; } = new();
        public decimal ContractualRevenueRisk { get; set; }
        public string RevenueRiskFormatted { get; set; } = string.Empty;
        public decimal CargoValue { get; set; }
        public decimal LiquidatedPenalty { get; set; }
        public int AffectedCustomerAccounts { get; set; }
        public string CustomerImpactSummary { get; set; } = string.Empty;
        public int RemainingSlaBufferMinutes { get; set; }
        public string RemainingSlaBufferFormatted { get; set; } = string.Empty;

        // Exposure Dossier for Affected Consignees
        public List<EntityExposureDto> ExposureDossier { get; set; } = new();

        // Hierarchical Causal Graph (Nodes & Edges for Frontend Canvas)
        public List<ImpactGraphNodeDto> GraphNodes { get; set; } = new();
        public List<ImpactGraphEdgeDto> GraphEdges { get; set; } = new();
    }

    public class BlastRadiusBreakdownDto
    {
        public int RootIncidents { get; set; } = 1;
        public int Shipments { get; set; }
        public int PurchaseOrders { get; set; }
        public int Consignees { get; set; }
    }

    public class EntityExposureDto
    {
        public string EntityId { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string Tier { get; set; } = string.Empty; // e.g. "Tier-1 Strategic Consignee"
        public string RiskLevel { get; set; } = string.Empty; // "CRITICAL", "HIGH RISK", "MEDIUM"
        public string Location { get; set; } = string.Empty;
        public string PoReference { get; set; } = string.Empty;
        public decimal ValueAtRisk { get; set; }
        public string ValueAtRiskFormatted { get; set; } = string.Empty;
        public int BufferRemainingMinutes { get; set; }
        public string BufferRemainingFormatted { get; set; } = string.Empty;
        public string SlaPenaltyRate { get; set; } = string.Empty;
        public string ClinicalOrBusinessImpact { get; set; } = string.Empty;
        public string AssignedCarrier { get; set; } = string.Empty;
        public string CargoDetails { get; set; } = string.Empty;
    }

    public class ImpactGraphNodeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int TierLevel { get; set; } // 0: Root, 1: Direct/Coupled/Facility, 2: PO/Internal, 3: End Consignees
        public string NodeType { get; set; } = string.Empty; // "IncidentOrigin", "DirectDisruption", "DownstreamPO", "EndConsignee", "Facility"
        public string Status { get; set; } = string.Empty; // "Critical Delay", "Stall", "Buffer Depleted"
        public string Subtitle { get; set; } = string.Empty;
        public Dictionary<string, string> Attributes { get; set; } = new();
    }

    public class ImpactGraphEdgeDto
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty; // "Triggers", "Carries", "Feeds", "CoupledWith"
        public string? Label { get; set; } // e.g., "Buffer 38m", "Cross-Dock"
        public string Severity { get; set; } = "High"; // "Critical", "High", "Warning"
    }

    public class ImpactSummaryDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public int TotalBlastRadius { get; set; }
        public string RevenueRiskFormatted { get; set; } = string.Empty;
        public int AffectedAccounts { get; set; }
        public string RemainingBufferFormatted { get; set; } = string.Empty;
    }
}
