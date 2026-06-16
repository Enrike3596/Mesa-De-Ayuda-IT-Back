using Data;
using DTOs;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolController : ControllerBase
    {
        private readonly IRolService _service;

        public RolController(IRolService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var roles = await _service.ObtenerTodosAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var rol = await _service.ObtenerPorIdAsync(id);
            if (rol == null) return NotFound(new { message = "Rol no encontrado" });
            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] RolCreateDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _service.EliminarAsync(id);
            if (!resultado) return NotFound(new { message = "Rol no encontrado" });
            return Ok(new { message = "Rol eliminado correctamente" });
        }
    }
}