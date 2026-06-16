using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubcategoriaController : ControllerBase
    {
        private readonly ISubcategoriaService _service;

        public SubcategoriaController(ISubcategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var subcategorias = await _service.ObtenerTodosAsync();
            return Ok(subcategorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var subcategoria = await _service.ObtenerPorIdAsync(id);
            if (subcategoria == null) return NotFound(new { message = "Subcategoría no encontrada" });
            return Ok(subcategoria);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SubcategoriaCreateDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SubcategoriaUpdateDTO dto)
        {
            var actualizado = await _service.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { message = "Subcategoría no encontrada" });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _service.EliminarAsync(id);
            if (!resultado) return NotFound(new { message = "Subcategoría no encontrada" });
            return Ok(new { message = "Subcategoría eliminada/desactivada correctamente" });
        }
    }
}