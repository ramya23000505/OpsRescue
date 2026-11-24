using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IAuthInterface
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> GetUserWithProfileAsync(int userId);
    }
}