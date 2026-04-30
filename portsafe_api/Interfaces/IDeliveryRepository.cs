using PortSafe.API.Models;

namespace PortSafe.API.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<IEnumerable<Delivery>> GetAllAsync();
        Task<Delivery?> GetByIdAsync(Guid id);
        Task<Delivery> CreateAsync(Delivery delivery);
        Task UpdateAsync(Delivery delivery);
        Task DeleteAsync(Delivery delivery);
    }
}
