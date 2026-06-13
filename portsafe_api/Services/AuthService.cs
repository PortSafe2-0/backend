using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly IEmailService _emailService;

        // Armazenamento em memória: email → (código, validade)
        private static readonly ConcurrentDictionary<string, (string Code, DateTime ExpiresAt)> _resetCodes = new();

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
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
                    CreatedAt = user.CreatedAt,
                    Phone = user.Phone,
                    Document = user.Document,
                    Block = user.Block,
                    UnitNumber = user.UnitNumber,
                    Street = user.Street,
                    HouseNumber = user.HouseNumber,
                    ZipCode = user.ZipCode
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

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            var code = new Random().Next(100000, 999999).ToString();
            _resetCodes[email.ToLower()] = (code, DateTime.UtcNow.AddMinutes(15));

            await _emailService.SendPasswordResetCodeAsync(email, code);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var key = email.ToLower();
            if (!_resetCodes.TryGetValue(key, out var entry)) return false;
            if (entry.Code != code || DateTime.UtcNow > entry.ExpiresAt) return false;

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            _resetCodes.TryRemove(key, out _);
            return true;
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            // Mapear a role do DTO para o enum Role
            var role = dto.Role?.ToLower() switch
            {
                "admin" => Role.Admin,
                "administrador" => Role.Admin,
                "porteiro" => Role.Porteiro,
                "morador" => Role.Morador,
                "residente" => Role.Morador,
                _ => Role.User
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = role,
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
                    CreatedAt = user.CreatedAt,
                    Phone = user.Phone,
                    Document = user.Document,
                    Block = user.Block,
                    UnitNumber = user.UnitNumber,
                    Street = user.Street,
                    HouseNumber = user.HouseNumber,
                    ZipCode = user.ZipCode
                }
            };
        }
    }


}
