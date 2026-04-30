using System;

namespace PortSafe.API.DTOs
{
    public class DeliveryResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LockerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? WithdrawnAt { get; set; }
    }
}