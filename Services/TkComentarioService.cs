using DTOs;
using Helpers;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Models;
using Repositories;

namespace Services
{
    public interface ITkComentarioService
    {
        Task<List<TkComentarioResponseDTO>> ObtenerTodosAsync();
        Task<TkComentarioResponseDTO?> ObtenerPorIdAsync(int id);
        Task<TkComentarioResponseDTO> CrearAsync(TkComentarioCreateDTO dto);
        Task<TkComentarioResponseDTO?> ActualizarAsync(int id, TkComentarioUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<List<TkComentarioResponseDTO>> ObtenerPorTicketAsync(int ticketId, bool puedeVerInternos);
    }

    public class TkComentarioService : ITkComentarioService
    {
        private readonly ITkComentarioRepository _repo;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly ITicketRepository _ticketRepo;

        public TkComentarioService(ITkComentarioRepository repo, IHubContext<TicketHub> hubContext, ITicketRepository ticketRepo)
        {
            _repo = repo;
            _hubContext = hubContext;
            _ticketRepo = ticketRepo;
        }

        public async Task<List<TkComentarioResponseDTO>> ObtenerTodosAsync()
        {
            var comentarios = await _repo.ObtenerTodosAsync();
            return comentarios.Select(MapearRespuesta).ToList();
        }

        public async Task<TkComentarioResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var comentario = await _repo.ObtenerPorIdAsync(id);
            return comentario == null ? null : MapearRespuesta(comentario);
        }

        public async Task<TkComentarioResponseDTO> CrearAsync(TkComentarioCreateDTO dto)
        {
            var comentario = new TkComentario
            {
                Comentario = dto.Comentario,
                UsuarioId = dto.UsuarioId,
                TicketId = dto.TicketId,
                FechaCreacion = DateTime.UtcNow,
                EsInterno = dto.EsInterno
            };

            var creado = await _repo.CrearAsync(comentario);

            // Notificar comentario nuevo
            await _hubContext.Clients.All.SendAsync("ComentarioNuevo", new
            {
                ticketId = dto.TicketId,
                comentario = MapearRespuesta(creado)
            });

            // Notificar ticket actualizado (cambió updated_at + comentarios)
            var ticket = await _ticketRepo.ObtenerPorIdAsync(dto.TicketId);
            if (ticket != null)
            {
                await _hubContext.Clients.All.SendAsync("TicketActualizado", MapearRespuestaTicket(ticket));
            }

            return MapearRespuesta(creado);
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

        public async Task<TkComentarioResponseDTO?> ActualizarAsync(int id, TkComentarioUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        public async Task<List<TkComentarioResponseDTO>> ObtenerPorTicketAsync(int ticketId, bool puedeVerInternos)
        {
            var comentarios = await _repo.ObtenerPorTicketAsync(ticketId);
            if (!puedeVerInternos)
            {
                comentarios = comentarios.Where(c => !c.EsInterno).ToList();
            }
            return comentarios.Select(MapearRespuesta).ToList();
        }

        private static TkComentarioResponseDTO MapearRespuesta(TkComentario c) => new()
        {
            Id = c.Id,
            Comentario = c.Comentario,
            UsuarioId = c.UsuarioId,
            TicketId = c.TicketId,
            EsInterno = c.EsInterno
        };
    }
}