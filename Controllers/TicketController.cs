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
	public class TicketController : ControllerBase
	{
		private readonly ITicketService _service;

		public TicketController(ITicketService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var tickets = await _service.ObtenerTodosAsync();
			return Ok(tickets);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(int id)
		{
			var ticket = await _service.ObtenerPorIdAsync(id);
			if (ticket == null) return NotFound(new { message = "Ticket no encontrado" });
			return Ok(ticket);
		}

		[HttpGet("{id}/sla")]
		public async Task<IActionResult> ConsultarSla(int id)
		{
			var sla = await _service.ConsultarSlaAsync(id);
			if (sla == null) return NotFound(new { message = "Ticket no encontrado" });
			return Ok(sla);
		}

		[HttpPost]
		public async Task<IActionResult> Crear([FromBody] TicketCreateDTO dto)
		{
			var creado = await _service.CrearAsync(dto);
			return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, [FromBody] TicketUpdateDTO dto)
		{
			var actualizado = await _service.ActualizarAsync(id, dto);
			if (actualizado == null) return NotFound(new { message = "Ticket no encontrado" });
			return Ok(actualizado);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			var resultado = await _service.EliminarAsync(id);
			if (!resultado) return NotFound(new { message = "Ticket no encontrado" });
			return Ok(new { message = "Ticket eliminado correctamente" });
		}

		[HttpPut("{id}/solicitar-cierre")]
		public async Task<IActionResult> SolicitarCierre(int id, [FromBody] SolicitudCierreDTO dto)
		{
			var agenteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			try
			{
				var ticket = await _service.SolicitarCierreAsync(id, agenteId, dto);
				if (ticket == null) return NotFound(new { message = "Ticket no encontrado" });
				return Ok(new { message = "Cierre solicitado. Esperando confirmación del usuario.", ticket });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPut("{id}/confirmar-cierre")]
		public async Task<IActionResult> ConfirmarCierre(int id, [FromBody] ConfirmacionCierreDTO dto)
		{
			var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			try
			{
				var ticket = await _service.ConfirmarCierreAsync(id, usuarioId, dto);
				if (ticket == null)
					return NotFound(new { message = "Ticket no encontrado o no tienes permiso." });

				var mensaje = dto.Aceptado
					? "Ticket cerrado exitosamente."
					: "Cierre rechazado. El ticket ha sido reabierto.";

				return Ok(new { message = mensaje, ticket });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("pendientes-confirmacion")]
		public async Task<IActionResult> PendientesConfirmacion()
		{
			var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
			var tickets = await _service.PendientesConfirmacionAsync(usuarioId);
			return Ok(tickets);
		}
	}
}
