using PortSafe.API.DTOs;

namespace PortSafe.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserCreateDto dto);
        
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
