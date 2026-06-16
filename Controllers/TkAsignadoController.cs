using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TkAsignadoController : ControllerBase
	{
		private readonly ITkAsignadoService _service;

		public TkAsignadoController(ITkAsignadoService service)
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
		public async Task<IActionResult> Crear([FromBody] TkAsignadoCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] TkAsignadoUpdateDTO dto)
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