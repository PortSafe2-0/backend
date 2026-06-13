using PortSafe.API.DTOs;

namespace PortSafe.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserCreateDto dto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);

        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
