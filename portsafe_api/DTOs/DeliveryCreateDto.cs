using System;
using System.ComponentModel.DataAnnotations;

namespace PortSafe.API.DTOs
{
    public class DeliveryCreateDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid LockerId { get; set; }
        [Required]
        public string RecipientName { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
    }
}