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
            CreatedAt = user.CreatedAt,
            Phone = user.Phone,
            Document = user.Document,
            Block = user.Block,
            UnitNumber = user.UnitNumber,
            Street = user.Street,
            HouseNumber = user.HouseNumber,
            ZipCode = user.ZipCode
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
                PasswordHash = dto.Password,
                Role = Enum.Parse<Role>(dto.Role, true),
                CreatedAt = DateTime.UtcNow,
                Phone = dto.Phone,
                Document = dto.Document,
                Block = dto.Block,
                UnitNumber = dto.UnitNumber,
                Street = dto.Street,
                HouseNumber = dto.HouseNumber,
                ZipCode = dto.ZipCode
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
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (dto.Document != null) user.Document = dto.Document;
            if (dto.Block != null) user.Block = dto.Block;
            if (dto.UnitNumber != null) user.UnitNumber = dto.UnitNumber;
            if (dto.Street != null) user.Street = dto.Street;
            if (dto.HouseNumber != null) user.HouseNumber = dto.HouseNumber;
            if (dto.ZipCode != null) user.ZipCode = dto.ZipCode;
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