using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Services
{
    public class LockerService : ILockerService
    {
        private readonly ILockerRepository _lockerRepository;
        public LockerService(ILockerRepository lockerRepository)
        {
            _lockerRepository = lockerRepository;
        }

        public async Task<IEnumerable<LockerResponseDto>> GetAllAsync()
        {
            var lockers = await _lockerRepository.GetAllAsync();
            return lockers.Select(l => new LockerResponseDto
            {
                Id = l.Id,
                Code = l.Code,
                Location = l.Location,
                Status = l.Status.ToString(),
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<LockerResponseDto?> GetByIdAsync(Guid id)
        {
            var locker = await _lockerRepository.GetByIdAsync(id);
            if (locker == null) return null;
            return new LockerResponseDto
            {
                Id = locker.Id,
                Code = locker.Code,
                Location = locker.Location,
                Status = locker.Status.ToString(),
                IsActive = locker.IsActive,
                CreatedAt = locker.CreatedAt
            };
        }

        public async Task<LockerResponseDto> CreateAsync(LockerCreateDto dto)
        {
            var locker = new Locker
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Location = dto.Location,
                Status = LockerStatus.Available,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _lockerRepository.CreateAsync(locker);
            return new LockerResponseDto
            {
                Id = locker.Id,
                Code = locker.Code,
                Location = locker.Location,
                Status = locker.Status.ToString(),
                IsActive = locker.IsActive,
                CreatedAt = locker.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(Guid id, LockerUpdateDto dto)
        {
            var locker = await _lockerRepository.GetByIdAsync(id);
            if (locker == null) return false;
            locker.Location = dto.Location;
            locker.IsActive = dto.IsActive;
            if (Enum.TryParse<LockerStatus>(dto.Status, out var status))
            {
                locker.Status = status;
            }
            await _lockerRepository.UpdateAsync(locker);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var locker = await _lockerRepository.GetByIdAsync(id);
            if (locker == null) return false;
            await _lockerRepository.DeleteAsync(locker);
            return true;
        }
    }
}
