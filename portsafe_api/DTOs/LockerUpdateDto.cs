using System.ComponentModel.DataAnnotations;

namespace PortSafe.API.DTOs
{
    public class LockerUpdateDto
    {
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; } = "Available";
    }
}
