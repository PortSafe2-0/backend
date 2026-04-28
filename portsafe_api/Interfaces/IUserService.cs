using PortSafe.API.DTOs;
using PortSafe.API.Models;

namespace PortSafe.API.Interfaces
{
    // Interface que define o contrato para o serviço de usuários
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync(); // Retorna todos os usuários cadastrados
        Task<UserResponseDto?> GetByIdAsync(Guid id); // Busca um usuário pelo ID
        Task<UserResponseDto?> GetByEmailAsync(string email); // Busca um usuário pelo e-mail
        Task<UserResponseDto> CreateAsync(UserCreateDto dto); // Cria um novo usuário
        Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateDto dto); // Atualiza um usuário existente
        Task<bool> DeleteAsync(Guid id); // Remove um usuário
    }
}