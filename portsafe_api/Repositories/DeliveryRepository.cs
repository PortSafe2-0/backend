using Microsoft.EntityFrameworkCore;
using PortSafe.API.Data;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly AppDbContext _context;
        public DeliveryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Delivery>> GetAllAsync()
        {
            return await _context.Deliveries
                .Include(d => d.User)
                .Include(d => d.Locker)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Delivery?> GetByIdAsync(Guid id)
        {
            return await _context.Deliveries
                .Include(d => d.User)
                .Include(d => d.Locker)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Delivery> CreateAsync(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task UpdateAsync(Delivery delivery)
        {
            _context.Deliveries.Update(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Delivery delivery)
        {
            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
        }
    }
}
