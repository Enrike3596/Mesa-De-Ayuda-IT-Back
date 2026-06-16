using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> ObtenerTodosAsync();
        Task<Ticket?> ObtenerPorIdAsync(int id);
        Task<Ticket?> ObtenerPorIdConPrioridadAsync(int id);
        Task<Ticket> CrearAsync(Ticket ticket);
        Task<Ticket?> ActualizarAsync(int id, TicketUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TicketRepository : ITicketRepository
    {
        private readonly DbContextcs _context;

        public TicketRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> ObtenerTodosAsync()
        {
            return await _context.Tickets
                .Include(t => t.Prioridad)
                .Include(t => t.Categoria)
                .Include(t => t.Subcategoria)
                .Include(t => t.TicketTipo)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Ticket?> ObtenerPorIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Prioridad)
                .Include(t => t.Categoria)
                .Include(t => t.Subcategoria)
                .Include(t => t.TicketTipo)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<Ticket?> ObtenerPorIdConPrioridadAsync(int id)
        {
            return _context.Tickets
                .Include(t => t.Prioridad)
                .Include(t => t.Categoria)
                .Include(t => t.Subcategoria)
                .Include(t => t.TicketTipo)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Ticket> CrearAsync(Ticket ticket)
        {
            ticket.Titulo = (ticket.Titulo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(ticket.Titulo))
                throw new ArgumentException("Título no puede ser vacío.", nameof(ticket));

            ticket.Descripcion = (ticket.Descripcion ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(ticket.Descripcion))
                throw new ArgumentException("Descripción no puede ser vacía.", nameof(ticket));

            // Defaults consistentes
            ticket.Estado = string.IsNullOrWhiteSpace(ticket.Estado)
                ? TicketEstadoNormalizer.Abierto
                : TicketEstadoNormalizer.Normalize(ticket.Estado);

            if (ticket.FechaCreacion == default)
                ticket.FechaCreacion = DateTime.UtcNow;

            // Valida TipoTicketId
            var tipoTicket = await _context.TipoTickets.FindAsync(ticket.TipoTicketId);
            if (tipoTicket == null)
                throw new ArgumentException("TipoTicketId no existe.", nameof(ticket.TipoTicketId));

            // Inicializa SLA según Prioridad (en horas)
            var prioridad = await _context.Prioridades.FindAsync(ticket.PrioridadId);
            if (prioridad == null)
                throw new ArgumentException("PrioridadId no existe.", nameof(ticket.PrioridadId));

            if (prioridad.Hora_sla > 0)
            {
                ticket.FechaLimiteSLA = ticket.FechaCreacion.AddHours(prioridad.Hora_sla);
                ticket.EstadoSLA = "En Tiempo";
                ticket.SLAVencido = false;
                ticket.TiempoAcumuladoPausaMinutos = 0;
                ticket.FechaPausaSLA = null;
            }
            else
            {
                // Sin SLA definido
                ticket.FechaLimiteSLA = null;
                ticket.EstadoSLA = null;
                ticket.SLAVencido = false;
                ticket.TiempoAcumuladoPausaMinutos = 0;
                ticket.FechaPausaSLA = null;
            }

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket?> ActualizarAsync(int id, TicketUpdateDTO dto)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Prioridad)
                .Include(t => t.Categoria)
                .Include(t => t.Subcategoria)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return null;

            var estadoAnterior = ticket.Estado;

            if (dto.Titulo != null)
            {
                var titulo = dto.Titulo.Trim();
                if (string.IsNullOrWhiteSpace(titulo))
                    throw new ArgumentException("Título no puede ser vacío.", nameof(dto.Titulo));
                ticket.Titulo = titulo;
            }

            if (dto.Descripcion != null)
            {
                var descripcion = dto.Descripcion.Trim();
                if (string.IsNullOrWhiteSpace(descripcion))
                    throw new ArgumentException("Descripción no puede ser vacía.", nameof(dto.Descripcion));
                ticket.Descripcion = descripcion;
            }

            if (dto.Estado != null)
            {
                var estado = TicketEstadoNormalizer.Normalize(dto.Estado);
                ticket.Estado = estado;

                if (estado == TicketEstadoNormalizer.Cerrado && ticket.FechaCierre == null)
                    ticket.FechaCierre = DateTime.UtcNow;

                if (estado == TicketEstadoNormalizer.Reabierto)
                {
                    ticket.FechaCierre = null;
                }
            }

            // Gestión SLA (pausa/reanuda/vencimiento) cuando cambia el estado
            if (dto.Estado != null && ticket.FechaLimiteSLA.HasValue)
            {
                var anterior = TicketEstadoNormalizer.Normalize(estadoAnterior);
                var actual = ticket.Estado;

                var estadosPausa = new[] { TicketEstadoNormalizer.EnEspera, TicketEstadoNormalizer.Programado, TicketEstadoNormalizer.PendienteConfirmacion };
                var estadosActivos = new[] { TicketEstadoNormalizer.Abierto, TicketEstadoNormalizer.EnProceso, TicketEstadoNormalizer.Reabierto };

                var ahora = DateTime.UtcNow;

                // Entra en pausa
                if (estadosPausa.Contains(actual) && !estadosPausa.Contains(anterior))
                {
                    ticket.FechaPausaSLA = ahora;
                    ticket.EstadoSLA = "Pausado";
                }

                // Reanuda desde pausa
                if (estadosActivos.Contains(actual) && ticket.FechaPausaSLA.HasValue)
                {
                    var minutosPausado = (int)(ahora - ticket.FechaPausaSLA.Value).TotalMinutes;
                    if (minutosPausado < 0) minutosPausado = 0;

                    ticket.TiempoAcumuladoPausaMinutos += minutosPausado;
                    ticket.FechaLimiteSLA = ticket.FechaLimiteSLA.Value.AddMinutes(minutosPausado);
                    ticket.FechaPausaSLA = null;
                    ticket.EstadoSLA = "En Tiempo";
                }

                // Cierre
                if (actual == TicketEstadoNormalizer.Cerrado)
                {
                    // Determina vencimiento al momento de cierre (si estaba pausado, no ajustamos nada aquí)
                    var vencido = ticket.SLAVencido || (ticket.FechaPausaSLA == null && ahora > ticket.FechaLimiteSLA.Value);
                    ticket.SLAVencido = vencido;
                    ticket.EstadoSLA = vencido ? "Resuelto Vencido" : "Resuelto En Tiempo";
                }
                else
                {
                    // Si no está pausado, recalcula vencimiento
                    if (ticket.FechaPausaSLA == null)
                    {
                        ticket.SLAVencido = ahora > ticket.FechaLimiteSLA.Value;
                        ticket.EstadoSLA = ticket.SLAVencido ? "Vencido" : (ticket.EstadoSLA == "Pausado" ? "En Tiempo" : (ticket.EstadoSLA ?? "En Tiempo"));
                    }
                }
            }

            if (dto.UsuarioId.HasValue) ticket.UsuarioCreadorId = dto.UsuarioId.Value;
            if (dto.CategoriaId.HasValue) ticket.CategoriaId = dto.CategoriaId.Value;
            if (dto.SubcategoriaId.HasValue) ticket.SubcategoriaId = dto.SubcategoriaId.Value;
            else if (dto.SubcategoriaId == null) ticket.SubcategoriaId = null;
            if (dto.PrioridadId.HasValue) ticket.PrioridadId = dto.PrioridadId.Value;
            if (dto.AreaId.HasValue) ticket.AreaId = dto.AreaId.Value;
            if (dto.TipoTicketId.HasValue)
            {
                var tipoTicket = await _context.TipoTickets.FindAsync(dto.TipoTicketId.Value);
                if (tipoTicket == null)
                    throw new ArgumentException("TipoTicketId no existe.", nameof(dto.TipoTicketId));
                ticket.TipoTicketId = dto.TipoTicketId.Value;
            }
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
