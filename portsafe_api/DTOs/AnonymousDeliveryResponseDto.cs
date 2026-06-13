namespace PortSafe.API.DTOs
{
    public class AnonymousDeliveryResponseDto
    {
        public Guid DeliveryId { get; set; }
        public string TrackingCode { get; set; } = string.Empty;
        public string LockerCode { get; set; } = string.Empty;
        public string LockerLocation { get; set; } = string.Empty;
    }
}
