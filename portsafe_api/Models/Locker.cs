using System;

namespace PortSafe.API.Models
{
	public enum LockerStatus
	{
		Available,
		Occupied,
		Maintenance
	}

	public class Locker
	{
		public Guid Id { get; set; }
		public string Code { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public LockerStatus Status { get; set; } = LockerStatus.Available;
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
