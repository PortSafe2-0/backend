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

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
                return Unauthorized(new { success = false, message = "Credenciais inválidas" });

            return Ok(new { success = true, token = result.Token, user = result.User });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });

            var result = await _authService.RegisterAsync(dto);

            if (result == null)
                return BadRequest(new { success = false, message = "Usuário já existe" });

            return Ok(new { success = true, token = result.Token, user = result.User });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "E-mail inválido" });

            // Sempre retorna sucesso para não revelar se o e-mail existe
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(new { success = true, message = "Se o e-mail estiver cadastrado, você receberá o código em instantes." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dados inválidos" });

            var ok = await _authService.ResetPasswordAsync(dto.Email, dto.Code, dto.NewPassword);
            if (!ok)
                return BadRequest(new { success = false, message = "Código inválido ou expirado" });

            return Ok(new { success = true, message = "Senha redefinida com sucesso" });
        }
    }
}