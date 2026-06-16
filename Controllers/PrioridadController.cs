using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PrioridadController : ControllerBase
	{
		private readonly IPrioridadService _service;

		public PrioridadController(IPrioridadService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var prioridades = await _service.ObtenerTodosAsync();
			return Ok(prioridades);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(int id)
		{
			var prioridad = await _service.ObtenerPorIdAsync(id);
			if (prioridad == null) return NotFound(new { message = "Prioridad no encontrada" });
			return Ok(prioridad);
		}

		[HttpPost]
		public async Task<IActionResult> Crear([FromBody] PrioridadCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] PrioridadUpdateDTO dto)
		{
			var actualizado = await _service.ActualizarAsync(id, dto);
			if (actualizado == null) return NotFound(new { message = "Prioridad no encontrada" });
			return Ok(actualizado);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			var resultado = await _service.EliminarAsync(id);
			if (!resultado) return NotFound(new { message = "Prioridad no encontrada" });
			return Ok(new { message = "Prioridad eliminada correctamente" });
		}
	}
}