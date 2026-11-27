using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IPolicyService
    {
        Task<PolicyEvaluationResultDto> EvaluatePolicyAsync(string incidentId, string strategyId);
        List<PolicyApprovalMatrixDto> GetApprovalMatrix();
    }
}
