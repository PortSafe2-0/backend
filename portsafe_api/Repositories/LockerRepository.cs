using Microsoft.EntityFrameworkCore;
using PortSafe.API.Data;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Repositories
{
    public class LockerRepository : ILockerRepository
    {
        private readonly AppDbContext _context;
        public LockerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Locker>> GetAllAsync()
        {
            return await _context.Lockers.AsNoTracking().ToListAsync();
        }

        public async Task<Locker?> GetByIdAsync(Guid id)
        {
            return await _context.Lockers.FindAsync(id);
        }

        public async Task<Locker?> GetByCodeAsync(string code)
        {
            return await _context.Lockers.FirstOrDefaultAsync(l => l.Code == code);
        }

        public async Task<Locker> CreateAsync(Locker locker)
        {
            _context.Lockers.Add(locker);
            await _context.SaveChangesAsync();
            return locker;
        }

        public async Task UpdateAsync(Locker locker)
        {
            _context.Lockers.Update(locker);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Locker locker)
        {
            _context.Lockers.Remove(locker);
            await _context.SaveChangesAsync();
        }
    }
}
