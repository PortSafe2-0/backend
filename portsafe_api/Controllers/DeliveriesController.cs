using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;

namespace PortSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        public DeliveriesController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var deliveries = await _deliveryService.GetAllAsync();
            return Ok(new { success = true, data = deliveries });
        }

        // GET /api/deliveries/my — retorna apenas as entregas do usuário autenticado
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { success = false, message = "Token inválido" });

            var all = await _deliveryService.GetAllAsync();
            var mine = all.Where(d => d.UserId == userId);
            return Ok(new { success = true, data = mine });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var delivery = await _deliveryService.GetByIdAsync(id);
            if (delivery == null)
                return NotFound(new { success = false, message = "Entrega não encontrada" });
            return Ok(new { success = true, data = delivery });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Porteiro")]
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();
                return BadRequest(new { success = false, message = string.Join(" | ", errors) });
            }
            try
            {
                var delivery = await _deliveryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = delivery.Id }, new { success = true, data = delivery });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Porteiro")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DeliveryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });
            var updated = await _deliveryService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(new { success = false, message = "Entrega não encontrada" });
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _deliveryService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Entrega não encontrada" });
            return Ok(new { success = true });
        }

        [HttpPost("anonymous")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAnonymous([FromBody] AnonymousDeliveryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });
            try
            {
                var result = await _deliveryService.CreateAnonymousAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/notify")]
        [Authorize(Roles = "Admin,Porteiro")]
        public async Task<IActionResult> Notify(Guid id)
        {
            var delivery = await _deliveryService.GetByIdAsync(id);
            if (delivery == null)
                return NotFound(new { success = false, message = "Entrega não encontrada" });
            return Ok(new { success = true, message = "Morador notificado com sucesso" });
        }

        [HttpPost("{id}/withdraw")]
        [Authorize]
        public async Task<IActionResult> Withdraw(Guid id)
        {
            try
            {
                var withdrawn = await _deliveryService.WithdrawAsync(id);
                if (!withdrawn)
                    return BadRequest(new { success = false, message = "Entrega não encontrada ou já retirada" });
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
