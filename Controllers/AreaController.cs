using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class AreaController : ControllerBase
	{
		private readonly IAreaService _service;

		public AreaController(IAreaService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var areas = await _service.ObtenerTodosAsync();
			return Ok(areas);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(int id)
		{
			var area = await _service.ObtenerPorIdAsync(id);
			if (area == null) return NotFound(new { message = "Área no encontrada" });
			return Ok(area);
		}

		[HttpPost]
		public async Task<IActionResult> Crear([FromBody] AreaCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] AreaUpdateDTO dto)
		{
			var actualizado = await _service.ActualizarAsync(id, dto);
			if (actualizado == null) return NotFound(new { message = "Área no encontrada" });
			return Ok(actualizado);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			var resultado = await _service.EliminarAsync(id);
			if (!resultado) return NotFound(new { message = "Área no encontrada" });
			return Ok(new { message = "Área eliminada/desactivada correctamente" });
		}
	}
}