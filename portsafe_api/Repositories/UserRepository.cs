using Microsoft.EntityFrameworkCore;
using PortSafe.API.Data;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Repositories
{
    // Implementação do repositório de usuários, responsável pelo acesso ao banco de dados
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        // Recebe o contexto do banco via injeção de dependência
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todos os usuários cadastrados
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Busca um usuário pelo ID
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Busca um usuário pelo e-mail
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Adiciona um novo usuário ao banco
        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Atualiza um usuário existente
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Remove um usuário do banco
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByNameAsync(string name)
        {
            return await _context.Users
                .Where(u => u.Role == Role.Morador)
                .FirstOrDefaultAsync(u => u.Name.ToLower().Contains(name.ToLower()));
        }
    }
}