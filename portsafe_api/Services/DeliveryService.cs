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

        public DeliveryService(IDeliveryRepository deliveryRepository, ILockerRepository lockerRepository, IUserRepository userRepository)
        {
            _deliveryRepository = deliveryRepository;
            _lockerRepository = lockerRepository;
            _userRepository = userRepository;
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
            // Validações
            var locker = await _lockerRepository.GetByIdAsync(dto.LockerId);
            if (locker == null || locker.Status != LockerStatus.Available || !locker.IsActive)
                throw new InvalidOperationException("Locker não disponível para entrega.");

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException("Usuário não encontrado.");

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

            // Locker para ocupado
            locker.Status = LockerStatus.Occupied;
            await _lockerRepository.UpdateAsync(locker);
            await _deliveryRepository.CreateAsync(delivery);
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

        public async Task<bool> WithdrawAsync(Guid id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            if (delivery == null || delivery.Status != DeliveryStatus.Delivered) return false;
            var locker = await _lockerRepository.GetByIdAsync(delivery.LockerId);
            if (locker == null) return false;
            delivery.Status = DeliveryStatus.Withdrawn;
            delivery.WithdrawnAt = DateTime.UtcNow;
            locker.Status = LockerStatus.Available;
            await _deliveryRepository.UpdateAsync(delivery);
            await _lockerRepository.UpdateAsync(locker);
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
