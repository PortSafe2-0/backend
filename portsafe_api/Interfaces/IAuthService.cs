using PortSafe.API.DTOs;

namespace PortSafe.API.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto loginDto);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
