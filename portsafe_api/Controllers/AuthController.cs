using Microsoft.AspNetCore.Mvc;
using PortSafe.API.DTOs;
using PortSafe.API.Interfaces;

namespace PortSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });

            var token = await _authService.LoginAsync(loginDto);

            if (token == null)
                return Unauthorized(new { success = false, message = "Credenciais inválidas" });

            return Ok(new { success = true, token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });

            var token = await _authService.RegisterAsync(dto);

            if (token == null)
                return BadRequest(new { success = false, message = "Usuário já existe" });

            return Ok(new { success = true, token });
        }
    }
}