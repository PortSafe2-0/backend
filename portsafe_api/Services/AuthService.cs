using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null) return null;

            if (!VerifyPassword(loginDto.Password, user.PasswordHash)) return null;

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public string HashPassword(string password)
        {
            // Use um hash seguro em produção! Aqui é só exemplo.
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // Use um hash seguro em produção! Aqui é só exemplo.
            return HashPassword(password) == passwordHash;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(double.Parse(jwtConfig["ExpiresInHours"]!));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = Role.User, // 🔐 IMPORTANTE
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                }
            };
        }
    }


}
