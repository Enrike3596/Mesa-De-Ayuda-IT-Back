using DTOs;
using Helpers;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Models;
using Repositories;

namespace Services
{
    public interface ITkAsignadoService
    {
        Task<List<TkAsignadoResponseDTO>> ObtenerTodosAsync();
        Task<TkAsignadoResponseDTO?> ObtenerPorIdAsync(int id);
        Task<TkAsignadoResponseDTO> CrearAsync(TkAsignadoCreateDTO dto);
        Task<TkAsignadoResponseDTO?> ActualizarAsync(int id, TkAsignadoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TkAsignadoService : ITkAsignadoService
    {
        private readonly ITkAsignadoRepository _repo;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly ITicketRepository _ticketRepo;

        public TkAsignadoService(ITkAsignadoRepository repo, IHubContext<TicketHub> hubContext, ITicketRepository ticketRepo)
        {
            _repo = repo;
            _hubContext = hubContext;
            _ticketRepo = ticketRepo;
        }

        public async Task<List<TkAsignadoResponseDTO>> ObtenerTodosAsync()
        {
            var asignados = await _repo.ObtenerTodosAsync();
            return asignados.Select(MapearRespuesta).ToList();
        }

        public async Task<TkAsignadoResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var asignado = await _repo.ObtenerPorIdAsync(id);
            return asignado == null ? null : MapearRespuesta(asignado);
        }

        public async Task<TkAsignadoResponseDTO> CrearAsync(TkAsignadoCreateDTO dto)
        {
            var asignado = new TkAsignado
            {
                TicketId = dto.TicketId,
                UsuarioAgenteId = dto.UsuarioId,
                FechaAsignacion = DateTime.UtcNow
            };

            var creado = await _repo.CrearAsync(asignado);

            // Notificar ticket actualizado (cambió la asignación)
            await NotificarTicketActualizado(dto.TicketId);

            return MapearRespuesta(creado);
        }

        public async Task<TkAsignadoResponseDTO?> ActualizarAsync(int id, TkAsignadoUpdateDTO dto)
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

        private static TkAsignadoResponseDTO MapearRespuesta(TkAsignado a) => new()
        {
            Id = a.Id,
            TicketId = a.TicketId,
            UsuarioId = a.UsuarioAgenteId
        };
    }
}