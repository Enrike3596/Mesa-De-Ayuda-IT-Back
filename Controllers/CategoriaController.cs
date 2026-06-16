using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CategoriaController : ControllerBase
	{
		private readonly ICategoriaService _service;

		public CategoriaController(ICategoriaService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var categorias = await _service.ObtenerTodosAsync();
			return Ok(categorias);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(int id)
		{
			var categoria = await _service.ObtenerPorIdAsync(id);
			if (categoria == null) return NotFound(new { message = "Categoría no encontrada" });
			return Ok(categoria);
		}

		[HttpPost]
		public async Task<IActionResult> Crear([FromBody] CategoriaCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] CategoriaUpdateDTO dto)
		{
			var actualizado = await _service.ActualizarAsync(id, dto);
			if (actualizado == null) return NotFound(new { message = "Categoría no encontrada" });
			return Ok(actualizado);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			var resultado = await _service.EliminarAsync(id);
			if (!resultado) return NotFound(new { message = "Categoría no encontrada" });
			return Ok(new { message = "Categoría eliminada/desactivada correctamente" });
		}
	}
}