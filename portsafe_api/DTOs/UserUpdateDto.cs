namespace PortSafe.API.DTOs
{
    public class UserUpdateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? Phone { get; set; }
        public string? Document { get; set; }
        public string? Block { get; set; }
        public string? UnitNumber { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? ZipCode { get; set; }
    }
}