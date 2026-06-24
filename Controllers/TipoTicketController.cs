using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoTicketController : ControllerBase
    {
        private readonly ITipoTicketService _service;

        public TipoTicketController(ITipoTicketService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var tipos = await _service.ObtenerTodosAsync();
            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var tipo = await _service.ObtenerPorIdAsync(id);
            if (tipo == null) return NotFound(new { message = "Tipo de ticket no encontrado" });
            return Ok(tipo);
        }
    }
}
