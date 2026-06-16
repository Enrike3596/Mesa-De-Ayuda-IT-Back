using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TkAnexoController : ControllerBase
    {
        private readonly ITkAnexoService _service;

        public TkAnexoController(ITkAnexoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos([FromQuery] int? ticketId = null)
        {
            var anexos = ticketId.HasValue
                ? await _service.ObtenerPorTicketIdAsync(ticketId.Value)
                : await _service.ObtenerTodosAsync();
            return Ok(anexos);
        }

        [HttpGet("ticket/{ticketId:int}")]
        public async Task<IActionResult> ObtenerPorTicketId([FromRoute] int ticketId)
        {
            var anexos = await _service.ObtenerPorTicketIdAsync(ticketId);
            return Ok(anexos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var anexo = await _service.ObtenerPorIdAsync(id);
            if (anexo == null) return NotFound(new { message = "Anexo no encontrado" });
            return Ok(anexo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] TkAnexoCreateDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            var form = await Request.ReadFormAsync();

            var file = form.Files.GetFile("file");
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "El archivo es obligatorio y no debe estar vacío." });

            static int? TryReadInt(IFormCollection f, string key)
            {
                var sv = f[key];
                if (sv.Count == 0) return null;
                return int.TryParse(sv[0], out var v) ? v : null;
            }

            var ticketId = TryReadInt(form, "TicketId") ?? TryReadInt(form, "ticketId");
            if (!ticketId.HasValue || ticketId.Value <= 0)
                return BadRequest(new { message = "TicketId es obligatorio y debe ser > 0." });

            var usuarioId = TryReadInt(form, "UsuarioId") ?? TryReadInt(form, "usuarioId");
            if (!usuarioId.HasValue || usuarioId.Value <= 0)
            {
                var claim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                usuarioId = int.TryParse(claim, out var v) ? v : null;
            }

            if (!usuarioId.HasValue || usuarioId.Value <= 0)
                return BadRequest(new { message = "UsuarioId es obligatorio y debe ser > 0." });

            var creado = await _service.SubirArchivoAsync(file, ticketId.Value, usuarioId.Value);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] TkAnexoUpdateDTO dto)
        {
            var actualizado = await _service.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { message = "Anexo no encontrado" });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _service.EliminarAsync(id);
            if (!resultado) return NotFound(new { message = "Anexo no encontrado" });
            return Ok(new { message = "Anexo eliminado correctamente" });
        }
    }
}
