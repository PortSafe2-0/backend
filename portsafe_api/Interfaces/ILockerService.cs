using PortSafe.API.DTOs;

namespace PortSafe.API.Interfaces
{
    public interface ILockerService
    {
        Task<IEnumerable<LockerResponseDto>> GetAllAsync();
        Task<LockerResponseDto?> GetByIdAsync(Guid id);
        Task<LockerResponseDto> CreateAsync(LockerCreateDto dto);
        Task<bool> UpdateAsync(Guid id, LockerUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
