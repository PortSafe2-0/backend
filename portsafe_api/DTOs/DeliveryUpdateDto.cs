using System;
using System.ComponentModel.DataAnnotations;

namespace PortSafe.API.DTOs
{
    public class DeliveryUpdateDto
    {
        [Required]
        public string RecipientName { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = "Pending";
    }
}