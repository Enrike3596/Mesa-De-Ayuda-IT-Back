using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TicketAsignadoController : ControllerBase
	{
		private readonly ITicketAsignadoService _service;

		public TicketAsignadoController(ITicketAsignadoService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var asignados = await _service.ObtenerTodosAsync();
			return Ok(asignados);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(int id)
		{
			var asignado = await _service.ObtenerPorIdAsync(id);
			if (asignado == null) return NotFound(new { message = "Asignación no encontrada" });
			return Ok(asignado);
		}

		[HttpPost]
		public async Task<IActionResult> Crear([FromBody] TicketAsignadoCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] TicketAsignadoUpdateDTO dto)
		{
			var actualizado = await _service.ActualizarAsync(id, dto);
			if (actualizado == null) return NotFound(new { message = "Asignación no encontrada" });
			return Ok(actualizado);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			var resultado = await _service.EliminarAsync(id);
			if (!resultado) return NotFound(new { message = "Asignación no encontrada" });
			return Ok(new { message = "Asignación eliminada correctamente" });
		}
	}
}