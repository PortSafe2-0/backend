using PortSafe.API.Models;

namespace PortSafe.API.Interfaces
{
    public interface ILockerRepository
    {
        Task<IEnumerable<Locker>> GetAllAsync();
        Task<Locker?> GetByIdAsync(Guid id);
        Task<Locker?> GetByCodeAsync(string code);
        Task<Locker> CreateAsync(Locker locker);
        Task UpdateAsync(Locker locker);
        Task DeleteAsync(Locker locker);
    }
}
