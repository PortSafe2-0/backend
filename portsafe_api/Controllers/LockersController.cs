using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortSafe.API.Data;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;
using PortSafe.API.Models;

namespace PortSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LockersController : ControllerBase
    {
        private readonly ILockerService _lockerService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LockersController(ILockerService lockerService, AppDbContext context, IWebHostEnvironment env)
        {
            _lockerService = lockerService;
            _context = context;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var lockers = await _lockerService.GetAllAsync();
            return Ok(new { success = true, data = lockers });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
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

        // DEV ONLY: reseta todos os lockers para Available
        [HttpPost("dev-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> DevReset()
        {
            if (!_env.IsDevelopment())
                return NotFound();
            var lockers = await _context.Lockers.ToListAsync();
            foreach (var l in lockers)
                l.Status = LockerStatus.Available;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = $"{lockers.Count} locker(s) resetados para Available." });
        }

        // DEV ONLY: insere os armários padrão se a tabela estiver vazia
        [HttpPost("dev-seed")]
        [AllowAnonymous]
        public async Task<IActionResult> DevSeed()
        {
            if (!_env.IsDevelopment())
                return NotFound();
            if (await _context.Lockers.AnyAsync())
                return Ok(new { success = true, message = "Armários já existem, nenhum inserido." });

            var defaults = new[]
            {
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"), Code = "A01", Location = "Portaria - Bloco A", Status = LockerStatus.Available, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"), Code = "A02", Location = "Portaria - Bloco A", Status = LockerStatus.Available, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"), Code = "A03", Location = "Portaria - Bloco A", Status = LockerStatus.Available, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004"), Code = "B01", Location = "Portaria - Bloco B", Status = LockerStatus.Available, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000005"), Code = "B02", Location = "Portaria - Bloco B", Status = LockerStatus.Available, IsActive = true, CreatedAt = DateTime.UtcNow },
            };

            _context.Lockers.AddRange(defaults);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = $"{defaults.Length} armários criados com sucesso." });
        }
    }
}
