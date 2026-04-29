using System.ComponentModel.DataAnnotations;

namespace PortSafe.API.DTOs
{
    public class LockerCreateDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
