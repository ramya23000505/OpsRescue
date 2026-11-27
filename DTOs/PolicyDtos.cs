using System.ComponentModel.DataAnnotations;

namespace OpsRescue.DTOs
{
    public class PolicyEvaluationResultDto
    {
        public string IncidentId { get; set; } = string.Empty;
        public string StrategyId { get; set; } = string.Empty;
        public string StrategyTitle { get; set; } = string.Empty;
        public bool IsEligibleForAutoApproval { get; set; }
        public string RequiredApprovalTier { get; set; } = "Tier 1";
        public string GovernanceVerdict { get; set; } = "Automated Dispatch Eligible";
        public decimal EstimatedInterventionCost { get; set; }
        public string EstimatedInterventionCostFormatted { get; set; } = string.Empty;
        public decimal ProtectedRevenueValue { get; set; }
        public string ProtectedRevenueValueFormatted { get; set; } = string.Empty;
        public List<PolicyRuleEvaluationItemDto> AppliedRules { get; set; } = new();
        public string RecommendedNextStep { get; set; } = string.Empty;
    }

    public class PolicyRuleEvaluationItemDto
    {
        public string RuleCode { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public string ImpactOnApproval { get; set; } = string.Empty;
    }

    public class PolicyApprovalMatrixDto
    {
        public string Tier { get; set; } = string.Empty;
        public string CostThreshold { get; set; } = string.Empty;
        public string RequiredRole { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExampleScenario { get; set; } = string.Empty;
    }

    public class PolicyApproveRequestDto
    {
        [Required]
        public string IncidentId { get; set; } = string.Empty;

        [Required]
        public string StrategyId { get; set; } = string.Empty;

        public string? ApprovalNotes { get; set; }
    }
}
