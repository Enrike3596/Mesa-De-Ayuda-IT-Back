using System.Security.Claims;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _service;

        public NotificacionController(INotificacionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var notificaciones = await _service.ListarPorUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        [HttpGet("no-leidas")]
        public async Task<IActionResult> ContarNoLeidas()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cantidad = await _service.ContarNoLeidasAsync(usuarioId);
            return Ok(new NotificacionCountDTO { Cantidad = cantidad });
        }

        [HttpPut("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.MarcarComoLeidaAsync(id, usuarioId);
            return Ok(new { message = "Notificación marcada como leída" });
        }

        [HttpPut("leer-todas")]
        public async Task<IActionResult> MarcarTodasComoLeidas()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.MarcarTodasComoLeidasAsync(usuarioId);
            return Ok(new { message = "Todas las notificaciones marcadas como leídas" });
        }
    }
}
