using DTOs;
using Helpers;
using Models;
using Repositories;

namespace Services
{
    public interface IAreaService
    {
        Task<List<AreaResponseDTO>> ObtenerTodosAsync();
        Task<AreaResponseDTO?> ObtenerPorIdAsync(int id);
        Task<AreaResponseDTO> CrearAsync(AreaCreateDTO dto);
        Task<AreaResponseDTO?> ActualizarAsync(int id, AreaUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _repo;

        public AreaService(IAreaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AreaResponseDTO>> ObtenerTodosAsync()
        {
            var areas = await _repo.ObtenerTodosAsync();
            return areas.Select(MapearRespuesta).ToList();
        }

        public async Task<AreaResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var area = await _repo.ObtenerPorIdAsync(id);
            return area == null ? null : MapearRespuesta(area);
        }

        public async Task<AreaResponseDTO> CrearAsync(AreaCreateDTO dto)
        {
            var area = new Area
            {
                NombreArea = dto.NombreArea,
                Estado = dto.Estado
            };

            var creado = await _repo.CrearAsync(area);
            return MapearRespuesta(creado);
        }

        public async Task<AreaResponseDTO?> ActualizarAsync(int id, AreaUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static AreaResponseDTO MapearRespuesta(Area a) => new()
        {
            Id = a.Id,
            NombreArea = a.NombreArea,
            Estado = a.Estado
        };
    }
}