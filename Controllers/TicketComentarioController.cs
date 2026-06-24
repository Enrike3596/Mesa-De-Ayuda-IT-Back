using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TicketComentarioController : ControllerBase
	{
		private readonly ITicketComentarioService _service;

		public TicketComentarioController(ITicketComentarioService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var comentarios = await _service.ObtenerTodosAsync();
			return Ok(comentarios);
		}

		[HttpGet("ticket/{ticketId}")]
		public async Task<IActionResult> ObtenerPorTicket(int ticketId)
		{
			var puedeVerInternos = User.IsInRole("Administrador") || User.IsInRole("Agente de soporte técnico");
			var comentarios = await _service.ObtenerPorTicketAsync(ticketId, puedeVerInternos);
			return Ok(comentarios);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(int id)
		{
			var comentario = await _service.ObtenerPorIdAsync(id);
			if (comentario == null) return NotFound(new { message = "Comentario no encontrado" });
			return Ok(comentario);
		}

		[HttpPost]
		public async Task<IActionResult> Crear([FromBody] TicketComentarioCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] TicketComentarioUpdateDTO dto)
		{
			var actualizado = await _service.ActualizarAsync(id, dto);
			if (actualizado == null) return NotFound(new { message = "Comentario no encontrado" });
			return Ok(actualizado);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			var resultado = await _service.EliminarAsync(id);
			if (!resultado) return NotFound(new { message = "Comentario no encontrado" });
			return Ok(new { message = "Comentario eliminado correctamente" });
		}
	}
}