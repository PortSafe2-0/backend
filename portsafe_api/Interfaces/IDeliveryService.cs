using PortSafe.API.DTOs;

namespace PortSafe.API.Interfaces
{
    public interface IDeliveryService
    {
        Task<IEnumerable<DeliveryResponseDto>> GetAllAsync();
        Task<DeliveryResponseDto?> GetByIdAsync(Guid id);
        Task<DeliveryResponseDto> CreateAsync(DeliveryCreateDto dto);
        Task<bool> UpdateAsync(Guid id, DeliveryUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> WithdrawAsync(Guid id);
        Task<AnonymousDeliveryResponseDto> CreateAnonymousAsync(AnonymousDeliveryCreateDto dto);
    }
}
