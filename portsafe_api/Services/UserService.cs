using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Services
{
    public class UserService : IUserService
    {
        // Repositório de usuários para acessar o banco
        private readonly IUserRepository _userRepository;

        // Construtor recebe o repositório via injeção de dependência
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Converte User para UserResponseDto (usado para resposta da API)
        private static UserResponseDto ToDto(User user) => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };

        // Retorna todos os usuários
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(ToDto);
        }

        // Busca usuário pelo ID
        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : ToDto(user);
        }

        // Busca usuário pelo e-mail
        public async Task<UserResponseDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : ToDto(user);
        }

        // Cria um novo usuário
        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = dto.Password, // Troque para hash real depois
                Role = Enum.Parse<Role>(dto.Role, true),
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.CreateAsync(user);
            return ToDto(user);
        }

        // Atualiza um usuário existente
        public async Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Role = Enum.Parse<Role>(dto.Role, true);
            await _userRepository.UpdateAsync(user);
            return ToDto(user);
        }

        // Remove um usuário
        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;
            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}