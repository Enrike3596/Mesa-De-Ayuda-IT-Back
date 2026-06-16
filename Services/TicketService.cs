using Data;
using DTOs;
using Helpers;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace Services
{
    public interface ITicketService
    {
        Task<List<TicketResponseDTO>> ObtenerTodosAsync();
        Task<TicketResponseDTO?> ObtenerPorIdAsync(int id);
        Task<TicketResponseDTO> CrearAsync(TicketCreateDTO dto);
        Task<TicketResponseDTO?> ActualizarAsync(int id, TicketUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<TicketSlaResponseDTO?> ConsultarSlaAsync(int id);
        Task<TicketResponseDTO?> SolicitarCierreAsync(int ticketId, int agenteId, SolicitudCierreDTO dto);
        Task<TicketResponseDTO?> ConfirmarCierreAsync(int ticketId, int usuarioId, ConfirmacionCierreDTO dto);
        Task<List<TicketResponseDTO>> PendientesConfirmacionAsync(int usuarioId);
    }

    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;
        private readonly DbContextcs _context;
        private readonly IHubContext<TicketHub> _hubContext;

        public TicketService(ITicketRepository repo, DbContextcs context, IHubContext<TicketHub> hubContext)
        {
            _repo = repo;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<TicketResponseDTO>> ObtenerTodosAsync()
        {
            var tickets = await _repo.ObtenerTodosAsync();
            return tickets.Select(MapearRespuesta).ToList();
        }

        public async Task<TicketResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var ticket = await _repo.ObtenerPorIdAsync(id);
            return ticket == null ? null : MapearRespuesta(ticket);
        }

        public async Task<TicketResponseDTO> CrearAsync(TicketCreateDTO dto)
        {
            var ticket = new Ticket
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Estado = TicketEstadoNormalizer.Abierto,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreadorId = dto.UsuarioId,
                CategoriaId = dto.CategoriaId,
                SubcategoriaId = dto.SubcategoriaId,
                PrioridadId = dto.PrioridadId,
                AreaId = dto.AreaId,
                TipoTicketId = dto.TipoTicketId
            };

            var creado = await _repo.CrearAsync(ticket);
            var response = MapearRespuesta(creado);

            await _hubContext.Clients.All.SendAsync("TicketCreado", response);

            return response;
        }

        public async Task<TicketResponseDTO?> ActualizarAsync(int id, TicketUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            if (actualizado == null) return null;

            var response = MapearRespuesta(actualizado);

            await _hubContext.Clients.All.SendAsync("TicketActualizado", response);

            return response;
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        public async Task<TicketSlaResponseDTO?> ConsultarSlaAsync(int id)
        {
            var ticket = await _repo.ObtenerPorIdConPrioridadAsync(id);
            if (ticket == null) return null;

            if (!ticket.FechaLimiteSLA.HasValue)
            {
                return new TicketSlaResponseDTO
                {
                    Id = ticket.Id,
                    FechaLimiteSLA = null,
                    EstadoSLA = ticket.EstadoSLA,
                    SLAVencido = false,
                    MinutosRestantes = 0,
                    TiempoVencidoMins = 0,
                    Prioridad = ticket.Prioridad?.Nombre,
                    HorasSLA = ticket.Prioridad?.Hora_sla ?? 0,
                    FechaPausaSLA = ticket.FechaPausaSLA,
                    TiempoAcumuladoPausaMinutos = ticket.TiempoAcumuladoPausaMinutos
                };
            }

            var ahora = DateTime.UtcNow;
            var minutos = (int)(ticket.FechaLimiteSLA.Value - ahora).TotalMinutes;
            var restantes = minutos > 0 ? minutos : 0;
            var vencidos = minutos < 0 ? Math.Abs(minutos) : 0;

            // Si está pausado, no forzamos vencimiento aquí (el deadline ya se extenderá al reanudar)
            var slaVencido = ticket.FechaPausaSLA == null && ahora > ticket.FechaLimiteSLA.Value;

            return new TicketSlaResponseDTO
            {
                Id = ticket.Id,
                FechaLimiteSLA = ticket.FechaLimiteSLA,
                EstadoSLA = ticket.EstadoSLA ?? (ticket.FechaPausaSLA != null ? "Pausado" : (slaVencido ? "Vencido" : "En Tiempo")),
                SLAVencido = ticket.SLAVencido || slaVencido,
                MinutosRestantes = restantes,
                TiempoVencidoMins = vencidos,
                Prioridad = ticket.Prioridad?.Nombre,
                HorasSLA = ticket.Prioridad?.Hora_sla ?? 0,
                FechaPausaSLA = ticket.FechaPausaSLA,
                TiempoAcumuladoPausaMinutos = ticket.TiempoAcumuladoPausaMinutos
            };
        }

        public async Task<TicketResponseDTO?> SolicitarCierreAsync(int ticketId, int agenteId, SolicitudCierreDTO dto)
        {
            var ticket = await _repo.ObtenerPorIdAsync(ticketId);
            if (ticket == null) return null;

            var estadosValidos = new[] { "En Proceso", "Abierto", "En Espera" };
            if (!estadosValidos.Contains(ticket.Estado))
                throw new InvalidOperationException($"No se puede solicitar cierre desde el estado: {ticket.Estado}");

            ticket.Estado = TicketEstadoNormalizer.PendienteConfirmacion;
            ticket.FechaSolicitudCierre = DateTime.UtcNow;
            ticket.CerradoPorUsuarioId = agenteId;

            var historial = new HistorialTicket
            {
                TicketId = ticketId,
                UsuarioId = agenteId,
                Descripcion = $"Cierre solicitado. Solución: {dto.ResumenSolucion}",
                FechaAccion = DateTime.UtcNow
            };

            _context.HistorialTickets.Add(historial);
            await _context.SaveChangesAsync();

            var response = MapearRespuesta(ticket);
            await _hubContext.Clients.All.SendAsync("TicketActualizado", response);
            return response;
        }

        public async Task<TicketResponseDTO?> ConfirmarCierreAsync(int ticketId, int usuarioId, ConfirmacionCierreDTO dto)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Prioridad)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.UsuarioCreadorId == usuarioId);

            if (ticket == null) return null;

            if (ticket.Estado != TicketEstadoNormalizer.PendienteConfirmacion)
                throw new InvalidOperationException("El ticket no está pendiente de confirmación.");

            if (dto.Aceptado)
            {
                ticket.Estado = TicketEstadoNormalizer.Cerrado;
                ticket.FechaCierre = DateTime.UtcNow;
                ticket.FechaConfirmacionCierre = DateTime.UtcNow;
                ticket.EstadoSLA = ticket.SLAVencido ? "Resuelto Vencido" : "Resuelto En Tiempo";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.MotivoRechazo))
                    throw new InvalidOperationException("Debe indicar el motivo del rechazo.");

                ticket.Estado = TicketEstadoNormalizer.Reabierto;
                ticket.FechaSolicitudCierre = null;
                ticket.FechaConfirmacionCierre = DateTime.UtcNow;
                ticket.MotivoRechazo = dto.MotivoRechazo;
            }

            var historial = new HistorialTicket
            {
                TicketId = ticketId,
                UsuarioId = usuarioId,
                Descripcion = dto.Aceptado
                    ? "Usuario aceptó el cierre del ticket."
                    : $"Usuario rechazó el cierre. Motivo: {dto.MotivoRechazo}",
                FechaAccion = DateTime.UtcNow
            };

            _context.HistorialTickets.Add(historial);
            await _context.SaveChangesAsync();

            var response = MapearRespuesta(ticket);
            await _hubContext.Clients.All.SendAsync("TicketActualizado", response);
            return response;
        }

        public async Task<List<TicketResponseDTO>> PendientesConfirmacionAsync(int usuarioId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.UsuarioCreadorId == usuarioId &&
                            t.Estado == TicketEstadoNormalizer.PendienteConfirmacion)
                .Include(t => t.Categoria)
                .Include(t => t.Prioridad)
                .AsNoTracking()
                .ToListAsync();

            return tickets.Select(MapearRespuesta).ToList();
        }

        private static TicketResponseDTO MapearRespuesta(Ticket t)
        {
            var ahora = DateTime.UtcNow;
            var slaVencido = t.SLAVencido;

            // Recalcula SLA vencido en tiempo real si el ticket tiene fecha límite y no está pausado/cerrado/resuelto
            if (t.FechaLimiteSLA.HasValue &&
                t.FechaPausaSLA == null &&
                t.Estado != "Cerrado" &&
                t.Estado != "Resuelto" &&
                ahora > t.FechaLimiteSLA.Value)
            {
                slaVencido = true;
            }

            return new TicketResponseDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Estado = t.Estado,
                UsuarioId = t.UsuarioCreadorId,
                CategoriaId = t.CategoriaId,
                SubcategoriaId = t.SubcategoriaId,
                PrioridadId = t.PrioridadId,
                AreaId = t.AreaId,
                TipoTicketId = t.TipoTicketId,
                FechaCreacion = t.FechaCreacion,
                FechaCierre = t.FechaCierre,
                FechaSolicitudCierre = t.FechaSolicitudCierre,
                FechaConfirmacionCierre = t.FechaConfirmacionCierre,
                MotivoRechazo = t.MotivoRechazo,
                CerradoPorUsuarioId = t.CerradoPorUsuarioId,
                FechaLimiteSLA = t.FechaLimiteSLA,
                EstadoSLA = t.EstadoSLA,
                SLAVencido = slaVencido,
                FechaPausaSLA = t.FechaPausaSLA,
                TiempoAcumuladoPausaMinutos = t.TiempoAcumuladoPausaMinutos,
                HorasSLA = t.Prioridad?.Hora_sla ?? 0,
                Categoria = t.Categoria != null
                    ? new CategoriaInfo { Id = t.Categoria.Id, Nombre = t.Categoria.Nombre }
                    : null,
                Subcategoria = t.Subcategoria != null
                    ? new SubcategoriaInfo { Id = t.Subcategoria.Id, NombreSubcategoria = t.Subcategoria.NombreSubcategoria }
                    : null,
                Prioridad = t.Prioridad != null
                    ? new PrioridadInfo { Id = t.Prioridad.Id, Nombre = t.Prioridad.Nombre, Hora_sla = t.Prioridad.Hora_sla }
                    : null,
                TipoTicket = t.TicketTipo != null
                    ? new TipoTicketInfo { Id = t.TicketTipo.Id, Nombre = t.TicketTipo.Nombre }
                    : null
            };
        }
    }
}
