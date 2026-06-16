using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CondominiosController : ControllerBase
    {
        private static readonly string[] _condominios = new[]
        {
            "Residencial Porto Seguro",
            "Condomínio Solar das Flores",
            "Edifício Vila Nova",
            "Condomínio Jardim das Acácias",
            "Residencial Bella Vista",
            "Condomínio Park Avenue",
            "Edifício Horizonte Azul",
            "Residencial Green Park",
        };

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(new { success = true, data = _condominios });
        }
    }
}
