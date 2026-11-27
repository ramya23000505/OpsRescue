using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IExecutionService
    {
        Task<DispatchExecutionResponseDto> DispatchExecutionAsync(DispatchExecutionRequestDto dto, string approvedBy);
        Task<RecoveryExecutionSummaryDto?> GetExecutionStatusAsync(string incidentId);
    }
}
