using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using portsafe_api.Services;
using portsafe_api.DTOs;

namespace portsafe_api.Controllers
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
            if (!result.Success)
                return Unauthorized(new { success = false, message = result.Message });

            return Ok(new { success = true, token = result.Token, user = result.User });
        }
    }
}
