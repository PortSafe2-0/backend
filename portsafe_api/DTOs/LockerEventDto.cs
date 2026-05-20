using System.Text.Json;

namespace PortSafe.API.DTOs
{
    public class LockerEventDto
    {
        public JsonElement LockerId { get; set; }
        public string? LockerCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; }
    }
}