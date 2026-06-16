using System.ComponentModel.DataAnnotations;

namespace PortSafe.API.DTOs
{
    public class AnonymousDeliveryCreateDto
    {
        [Required]
        public string RecipientName { get; set; } = string.Empty;
        public string? TrackingCode { get; set; }
    }
}
