using DTOs;
using Helpers;
using Models;
using Repositories;

namespace Services
{
    public interface IPrioridadService
    {
        Task<List<PrioridadResponseDTO>> ObtenerTodosAsync();
        Task<PrioridadResponseDTO?> ObtenerPorIdAsync(int id);
        Task<PrioridadResponseDTO> CrearAsync(PrioridadCreateDTO dto);
        Task<PrioridadResponseDTO?> ActualizarAsync(int id, PrioridadUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class PrioridadService : IPrioridadService
    {
        private readonly IPrioridadRepository _repo;

        public PrioridadService(IPrioridadRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PrioridadResponseDTO>> ObtenerTodosAsync()
        {
            var prioridades = await _repo.ObtenerTodosAsync();
            return prioridades.Select(MapearRespuesta).ToList();
        }

        public async Task<PrioridadResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var prioridad = await _repo.ObtenerPorIdAsync(id);
            return prioridad == null ? null : MapearRespuesta(prioridad);
        }

        public async Task<PrioridadResponseDTO> CrearAsync(PrioridadCreateDTO dto)
        {
            var prioridad = new Prioridad
            {
                Nombre = dto.NombrePrioridad,
                Tipo = dto.Tipo,
                Hora_sla = dto.Hora_sla,
                Estado   = dto.Estado
            };

            var creado = await _repo.CrearAsync(prioridad);
            return MapearRespuesta(creado);
        }

        public async Task<PrioridadResponseDTO?> ActualizarAsync(int id, PrioridadUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static PrioridadResponseDTO MapearRespuesta(Prioridad p) => new()
        {
            Id = p.Id,
            NombrePrioridad = p.Nombre,
            Tipo = p.Tipo,
            Hora_sla = p.Hora_sla,
            Estado = p.Estado
        };
    }
}
