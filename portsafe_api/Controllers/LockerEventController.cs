using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;
using System.Text.Json;

namespace PortSafe.API.Controllers
{
    [ApiController]
    [Route("api/locker")]
    public class LockerEventController : ControllerBase
    {
        private readonly ILockerService _lockerService;

        public LockerEventController(ILockerService lockerService)
        {
            _lockerService = lockerService;
        }

        [HttpPost("event")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveEvent([FromBody] LockerEventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Campo 'status' é obrigatório"
                });
            }

            var lockerIdentifier = ResolveLockerIdentifier(dto);
            if (string.IsNullOrWhiteSpace(lockerIdentifier))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Informe 'lockerId' (GUID/número) ou 'lockerCode'"
                });
            }

            var updated = await _lockerService.UpdateStatusFromEventAsync(lockerIdentifier, dto.Status);
            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Locker não encontrado para o identificador informado",
                    lockerIdentifier
                });
            }

            return Ok(new
            {
                success = true,
                message = "Evento processado com sucesso",
                data = new
                {
                    lockerIdentifier,
                    status = dto.Status,
                    receivedAt = DateTime.UtcNow,
                    sourceTimestamp = dto.Timestamp
                }
            });
        }

        private static string? ResolveLockerIdentifier(LockerEventDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.LockerCode))
            {
                return dto.LockerCode.Trim();
            }

            if (dto.LockerId.ValueKind == JsonValueKind.String)
            {
                return dto.LockerId.GetString()?.Trim();
            }

            if (dto.LockerId.ValueKind == JsonValueKind.Number)
            {
                return dto.LockerId.GetRawText();
            }

            return null;
        }
    }
}