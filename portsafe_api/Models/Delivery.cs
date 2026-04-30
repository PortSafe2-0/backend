using System;

namespace PortSafe.API.Models
{
    public enum DeliveryStatus
    {
        Pending,
        Delivered,
        Withdrawn,
        Cancelled
    }

    public class Delivery
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LockerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }
        public DateTime? WithdrawnAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Locker? Locker { get; set; }
    }
}