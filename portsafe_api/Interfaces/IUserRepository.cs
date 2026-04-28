using PortSafe.API.Models;

namespace PortSafe.API.Interfaces
{
    // Interface que define o contrato para o repositório de usuários
    public interface IUserRepository
    {   
        Task<IEnumerable<User>> GetAllAsync(); // Retorna todos os usuários cadastrados
        Task<User?> GetByIdAsync(Guid id); // Busca um usuário pelo ID
        Task<User?> GetByEmailAsync(string email); // Busca um usuário pelo e-mail
        Task CreateAsync(User user); // Adiciona um novo usuário
        Task UpdateAsync(User user); // Atualiza um usuário existente
        Task DeleteAsync(User user); // Remove um usuário
    }
}