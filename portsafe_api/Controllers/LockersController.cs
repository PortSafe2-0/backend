using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;

namespace PortSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LockersController : ControllerBase
    {
        private readonly ILockerService _lockerService;
        public LockersController(ILockerService lockerService)
        {
            _lockerService = lockerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lockers = await _lockerService.GetAllAsync();
            return Ok(new { success = true, data = lockers });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var locker = await _lockerService.GetByIdAsync(id);
            if (locker == null)
                return NotFound(new { success = false, message = "Locker não encontrado" });
            return Ok(new { success = true, data = locker });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] LockerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });
            var locker = await _lockerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = locker.Id }, new { success = true, data = locker });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LockerUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });
            var updated = await _lockerService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(new { success = false, message = "Locker não encontrado" });
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _lockerService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Locker não encontrado" });
            return Ok(new { success = true });
        }
    }
}
