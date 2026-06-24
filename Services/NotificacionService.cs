using Data;
using DTOs;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services
{
    public interface INotificacionService
    {
        Task<NotificacionResponseDTO> CrearNotificacionAsync(int usuarioId, int ticketId, string tipo, string mensaje);
        Task CrearNotificacionParaUsuariosAsync(List<int> usuarioIds, int ticketId, string tipo, string mensaje);
        Task CrearNotificacionTicketCreadoAsync(Ticket ticket);
        Task CrearNotificacionTicketAsignadoAsync(int ticketId, int agenteId);
        Task CrearNotificacionTicketActualizadoAsync(Ticket ticket, int? usuarioAccionId = null);
        Task CrearNotificacionComentarioNuevoAsync(TicketComentario comentario);
        Task<List<NotificacionResponseDTO>> ListarPorUsuarioAsync(int usuarioId);
        Task<int> ContarNoLeidasAsync(int usuarioId);
        Task MarcarComoLeidaAsync(int id, int usuarioId);
        Task MarcarTodasComoLeidasAsync(int usuarioId);
    }

    public class NotificacionService : INotificacionService
    {
        private readonly DbContextcs _context;
        private readonly IHubContext<TicketHub> _hubContext;

        public NotificacionService(DbContextcs context, IHubContext<TicketHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<NotificacionResponseDTO> CrearNotificacionAsync(int usuarioId, int ticketId, string tipo, string mensaje)
        {
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                TicketId = ticketId,
                Tipo = tipo,
                Mensaje = mensaje,
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            var response = new NotificacionResponseDTO
            {
                Id = notificacion.Id,
                UsuarioId = notificacion.UsuarioId,
                TicketId = notificacion.TicketId,
                Tipo = notificacion.Tipo,
                Mensaje = notificacion.Mensaje,
                Leida = notificacion.Leida,
                FechaCreacion = notificacion.FechaCreacion
            };

            await _hubContext.Clients.Group($"user_{usuarioId}").SendAsync("NotificacionCreada", response);

            return response;
        }

        public async Task CrearNotificacionParaUsuariosAsync(List<int> usuarioIds, int ticketId, string tipo, string mensaje)
        {
            var ahora = DateTime.UtcNow;
            var notificaciones = usuarioIds.Select(uid => new Notificacion
            {
                UsuarioId = uid,
                TicketId = ticketId,
                Tipo = tipo,
                Mensaje = mensaje,
                Leida = false,
                FechaCreacion = ahora
            }).ToList();

            _context.Notificaciones.AddRange(notificaciones);
            await _context.SaveChangesAsync();

            foreach (var usuarioId in usuarioIds)
            {
                var notif = notificaciones.First(n => n.UsuarioId == usuarioId);
                var response = new NotificacionResponseDTO
                {
                    Id = notif.Id,
                    UsuarioId = notif.UsuarioId,
                    TicketId = notif.TicketId,
                    Tipo = notif.Tipo,
                    Mensaje = notif.Mensaje,
                    Leida = notif.Leida,
                    FechaCreacion = notif.FechaCreacion
                };

                await _hubContext.Clients.Group($"user_{usuarioId}").SendAsync("NotificacionCreada", response);
            }
        }

        public async Task CrearNotificacionTicketCreadoAsync(Ticket ticket)
        {
            var rolesNotificar = new[] { "Administrador", "Agente de soporte técnico" };
            var usuarios = await _context.Usuarios
                .Where(u => u.Estado && rolesNotificar.Contains(u.Rol.Nombre))
                .ToListAsync();

            var usuarioIds = usuarios.Select(u => u.Id).ToList();
            if (usuarioIds.Count == 0) return;

            await CrearNotificacionParaUsuariosAsync(
                usuarioIds,
                ticket.Id,
                "TICKET_CREADO",
                $"Nuevo ticket creado: {ticket.Titulo}"
            );
        }

        public async Task CrearNotificacionTicketAsignadoAsync(int ticketId, int agenteId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return;

            await CrearNotificacionAsync(
                agenteId,
                ticketId,
                "TICKET_ASIGNADO",
                $"Se te ha asignado el ticket: {ticket.Titulo}"
            );
        }

        public async Task CrearNotificacionTicketActualizadoAsync(Ticket ticket, int? usuarioAccionId = null)
        {
            var destinatarios = new List<int>();

            destinatarios.Add(ticket.UsuarioCreadorId);

            var asignados = await _context.TicketAsignados
                .Where(ta => ta.TicketId == ticket.Id)
                .Select(ta => ta.UsuarioAgenteId)
                .ToListAsync();

            destinatarios.AddRange(asignados);

            if (usuarioAccionId.HasValue)
                destinatarios = destinatarios.Where(d => d != usuarioAccionId.Value).Distinct().ToList();
            else
                destinatarios = destinatarios.Distinct().ToList();

            if (destinatarios.Count == 0) return;

            var mensaje = $"El ticket ha sido actualizado: {ticket.Titulo}";
            await CrearNotificacionParaUsuariosAsync(destinatarios, ticket.Id, "TICKET_ACTUALIZADO", mensaje);
        }

        public async Task CrearNotificacionComentarioNuevoAsync(TicketComentario comentario)
        {
            var ticket = await _context.Tickets.FindAsync(comentario.TicketId);
            if (ticket == null) return;

            var destinatarios = new List<int>();

            destinatarios.Add(ticket.UsuarioCreadorId);

            var asignados = await _context.TicketAsignados
                .Where(ta => ta.TicketId == comentario.TicketId)
                .Select(ta => ta.UsuarioAgenteId)
                .ToListAsync();

            destinatarios.AddRange(asignados);

            destinatarios = destinatarios.Where(d => d != comentario.UsuarioId).Distinct().ToList();

            if (destinatarios.Count == 0) return;

            var usuario = await _context.Usuarios.FindAsync(comentario.UsuarioId);
            var nombreUsuario = usuario?.Nombre ?? "Usuario";
            var mensaje = $"{nombreUsuario} comentó en: {ticket.Titulo}";

            await CrearNotificacionParaUsuariosAsync(
                destinatarios,
                comentario.TicketId,
                "COMENTARIO_NUEVO",
                mensaje
            );
        }

        public async Task<List<NotificacionResponseDTO>> ListarPorUsuarioAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.FechaCreacion)
                .Select(n => new NotificacionResponseDTO
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    TicketId = n.TicketId,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    FechaCreacion = n.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<int> ContarNoLeidasAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);
        }

        public async Task MarcarComoLeidaAsync(int id, int usuarioId)
        {
            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);

            if (notificacion != null)
            {
                notificacion.Leida = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarcarTodasComoLeidasAsync(int usuarioId)
        {
            var noLeidas = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .ToListAsync();

            foreach (var n in noLeidas)
            {
                n.Leida = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
