using Microsoft.EntityFrameworkCore;
using PortSafe.API.Data;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly ILockerRepository _lockerRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public DeliveryService(IDeliveryRepository deliveryRepository, ILockerRepository lockerRepository, IUserRepository userRepository, AppDbContext context)
        {
            _deliveryRepository = deliveryRepository;
            _lockerRepository = lockerRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<DeliveryResponseDto>> GetAllAsync()
        {
            var deliveries = await _deliveryRepository.GetAllAsync();
            return deliveries.Select(MapToResponseDto);
        }

        public async Task<DeliveryResponseDto?> GetByIdAsync(Guid id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            return delivery == null ? null : MapToResponseDto(delivery);
        }

        public async Task<DeliveryResponseDto> CreateAsync(DeliveryCreateDto dto)
        {
            var locker = await _lockerRepository.GetByIdAsync(dto.LockerId);
            if (locker == null || locker.Status != LockerStatus.Available || !locker.IsActive)
                throw new InvalidOperationException($"Armário não disponível (status atual: {locker?.Status.ToString() ?? "não encontrado"}).");

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException($"Usuário não encontrado (id: {dto.UserId}).");

            var delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                LockerId = dto.LockerId,
                RecipientName = dto.RecipientName,
                TrackingCode = dto.TrackingCode,
                Status = DeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                locker.Status = LockerStatus.Occupied;
                _context.Lockers.Update(locker);
                _context.Deliveries.Add(delivery);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return MapToResponseDto(delivery);
        }

        public async Task<bool> UpdateAsync(Guid id, DeliveryUpdateDto dto)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            if (delivery == null) return false;
            delivery.RecipientName = dto.RecipientName;
            delivery.TrackingCode = dto.TrackingCode;
            if (Enum.TryParse<DeliveryStatus>(dto.Status, out var status))
            {
                delivery.Status = status;
            }
            await _deliveryRepository.UpdateAsync(delivery);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            if (delivery == null) return false;
            await _deliveryRepository.DeleteAsync(delivery);
            return true;
        }

        public async Task<AnonymousDeliveryResponseDto> CreateAnonymousAsync(AnonymousDeliveryCreateDto dto)
        {
            var user = await _userRepository.GetByNameAsync(dto.RecipientName);
            if (user == null)
                throw new InvalidOperationException($"Morador não encontrado com o nome \"{dto.RecipientName}\". Verifique e tente novamente.");

            var locker = await _lockerRepository.GetFirstAvailableAsync();
            if (locker == null)
                throw new InvalidOperationException("Nenhum armário disponível no momento. Tente mais tarde.");

            var trackingCode = dto.TrackingCode ?? $"PS{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            var delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                LockerId = locker.Id,
                RecipientName = dto.RecipientName,
                TrackingCode = trackingCode,
                Status = DeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                locker.Status = LockerStatus.Occupied;
                _context.Lockers.Update(locker);
                _context.Deliveries.Add(delivery);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return new AnonymousDeliveryResponseDto
            {
                DeliveryId = delivery.Id,
                TrackingCode = delivery.TrackingCode,
                LockerCode = locker.Code,
                LockerLocation = locker.Location
            };
        }

        public async Task<bool> WithdrawAsync(Guid id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null || delivery.Status == DeliveryStatus.Withdrawn || delivery.Status == DeliveryStatus.Cancelled) return false;
            var locker = await _context.Lockers.FindAsync(delivery.LockerId);
            if (locker == null) return false;
            delivery.Status = DeliveryStatus.Withdrawn;
            delivery.WithdrawnAt = DateTime.UtcNow;
            locker.Status = LockerStatus.Available;
            await _context.SaveChangesAsync();
            return true;
        }

        private DeliveryResponseDto MapToResponseDto(Delivery d)
        {
            return new DeliveryResponseDto
            {
                Id = d.Id,
                UserId = d.UserId,
                LockerId = d.LockerId,
                RecipientName = d.RecipientName,
                TrackingCode = d.TrackingCode,
                Status = d.Status.ToString(),
                CreatedAt = d.CreatedAt,
                DeliveredAt = d.DeliveredAt,
                WithdrawnAt = d.WithdrawnAt
            };
        }
    }
}
