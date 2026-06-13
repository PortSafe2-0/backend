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
    public class UsersController : ControllerBase
    {
        // Serviço de usuários, responsável pela lógica de negócio
        private readonly IUserService _userService;

        // Construtor recebe o serviço via injeção de dependência
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/users/me — retorna o usuário autenticado extraindo o ID do JWT
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { success = false, message = "Token inválido" });

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound(new { success = false, message = "Usuário não encontrado" });
            return Ok(new { success = true, data = user });
        }

        // GET  - Lista todos os usuários
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET - Busca usuário por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST - Cria um novo usuário
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT - Atualiza um usuário existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UserUpdateDto dto)
        {
            var user = await _userService.UpdateAsync(id, dto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // DELETE - Remove um usuário
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}