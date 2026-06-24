using DTOs;
using Helpers;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Models;
using Repositories;

namespace Services
{
    public interface ITicketAsignadoService
    {
        Task<List<TicketAsignadoResponseDTO>> ObtenerTodosAsync();
        Task<TicketAsignadoResponseDTO?> ObtenerPorIdAsync(int id);
        Task<TicketAsignadoResponseDTO> CrearAsync(TicketAsignadoCreateDTO dto);
        Task<TicketAsignadoResponseDTO?> ActualizarAsync(int id, TicketAsignadoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TicketAsignadoService : ITicketAsignadoService
    {
        private readonly ITicketAsignadoRepository _repo;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly ITicketRepository _ticketRepo;
        private readonly INotificacionService _notificacionService;

        public TicketAsignadoService(ITicketAsignadoRepository repo, IHubContext<TicketHub> hubContext, ITicketRepository ticketRepo, INotificacionService notificacionService)
        {
            _repo = repo;
            _hubContext = hubContext;
            _ticketRepo = ticketRepo;
            _notificacionService = notificacionService;
        }

        public async Task<List<TicketAsignadoResponseDTO>> ObtenerTodosAsync()
        {
            var asignados = await _repo.ObtenerTodosAsync();
            return asignados.Select(MapearRespuesta).ToList();
        }

        public async Task<TicketAsignadoResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var asignado = await _repo.ObtenerPorIdAsync(id);
            return asignado == null ? null : MapearRespuesta(asignado);
        }

        public async Task<TicketAsignadoResponseDTO> CrearAsync(TicketAsignadoCreateDTO dto)
        {
            var asignado = new TicketAsignado
            {
                TicketId = dto.TicketId,
                UsuarioAgenteId = dto.UsuarioId,
                FechaAsignacion = DateTime.UtcNow
            };

            var creado = await _repo.CrearAsync(asignado);

            // Notificar ticket actualizado (cambió la asignación)
            await NotificarTicketActualizado(dto.TicketId);
            await _notificacionService.CrearNotificacionTicketAsignadoAsync(dto.TicketId, dto.UsuarioId);

            return MapearRespuesta(creado);
        }

        public async Task<TicketAsignadoResponseDTO?> ActualizarAsync(int id, TicketAsignadoUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            // Obtener ticketId antes de eliminar para poder notificar después
            var asignado = await _repo.ObtenerPorIdAsync(id);
            var ticketId = asignado?.TicketId;

            var resultado = await _repo.EliminarAsync(id);

            if (resultado && ticketId.HasValue)
            {
                await NotificarTicketActualizado(ticketId.Value);
            }

            return resultado;
        }

        private async Task NotificarTicketActualizado(int ticketId)
        {
            var ticket = await _ticketRepo.ObtenerPorIdAsync(ticketId);
            if (ticket != null)
            {
                await _hubContext.Clients.All.SendAsync("TicketActualizado", MapearRespuestaTicket(ticket));
            }
        }

        private static TicketResponseDTO MapearRespuestaTicket(Models.Ticket t)
        {
            var ahora = DateTime.UtcNow;
            var slaVencido = t.SLAVencido;

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
                    : null
            };
        }

        private static TicketAsignadoResponseDTO MapearRespuesta(TicketAsignado a) => new()
        {
            Id = a.Id,
            TicketId = a.TicketId,
            UsuarioId = a.UsuarioAgenteId
        };
    }
}