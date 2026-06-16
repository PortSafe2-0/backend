using System;

namespace PortSafe.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }

        // Campos extras do cadastro
        public string? Phone { get; set; }
        public string? Document { get; set; }   // CPF
        public string? Block { get; set; }       // Bloco
        public string? UnitNumber { get; set; }  // Número do apartamento
        public string? Street { get; set; }      // Rua (casa)
        public string? HouseNumber { get; set; } // Número da casa
        public string? ZipCode { get; set; }     // CEP
    }
}