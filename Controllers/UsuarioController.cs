using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // todos los endpoints requieren token JWT
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _service.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var usuario = await _service.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(usuario);
        }

        [HttpPost]
        [AllowAnonymous] // registro no requiere token
        public async Task<IActionResult> Crear([FromBody] UsuarioCreateDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UsuarioUpdateDTO dto)
        {
            var actualizado = await _service.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(actualizado);
        }

        [HttpGet("area/{areaId}")]
        public async Task<IActionResult> ObtenerPorArea(int areaId)
        {
            var usuarios = await _service.ObtenerPorAreaAsync(areaId);
            return Ok(usuarios);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _service.EliminarAsync(id);
            if (!resultado) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(new { message = "Usuario desactivado correctamente" });
        }
    }
}