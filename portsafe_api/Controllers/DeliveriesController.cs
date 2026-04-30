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
                return BadRequest(new { success = false, message = "Dados inválidos" });
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

        [HttpPost("{id}/withdraw")]
        [Authorize(Roles = "Admin,Porteiro")]
        public async Task<IActionResult> Withdraw(Guid id)
        {
            var withdrawn = await _deliveryService.WithdrawAsync(id);
            if (!withdrawn)
                return BadRequest(new { success = false, message = "Não foi possível retirar a entrega" });
            return Ok(new { success = true });
        }
    }
}
