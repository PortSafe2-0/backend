using System;

namespace PortSafe.API.DTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; } // Guid = gerar indentificador único p/ cada usuário
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } // DateTime = data e hora de criação do usuário
    }
}