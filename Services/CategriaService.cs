using DTOs;
using Helpers;
using Models;
using Repositories;

namespace Services
{
    public interface ICategoriaService
    {
        Task<List<CategoriaResponseDTO>> ObtenerTodosAsync();
        Task<CategoriaResponseDTO?> ObtenerPorIdAsync(int id);
        Task<CategoriaResponseDTO> CrearAsync(CategoriaCreateDTO dto);
        Task<CategoriaResponseDTO?> ActualizarAsync(int id, CategoriaUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repo;

        public CategoriaService(ICategoriaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CategoriaResponseDTO>> ObtenerTodosAsync()
        {
            var categorias = await _repo.ObtenerTodosAsync();
            return categorias.Select(MapearRespuesta).ToList();
        }

        public async Task<CategoriaResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var categoria = await _repo.ObtenerPorIdAsync(id);
            return categoria == null ? null : MapearRespuesta(categoria);
        }

        public async Task<CategoriaResponseDTO> CrearAsync(CategoriaCreateDTO dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.NombreCategoria,
                Descripcion = dto.Descripcion,
                AreaId = dto.AreaId,
                TipoTicketId = dto.TipoTicketId,
                Estado = dto.Estado
            };

            var creado = await _repo.CrearAsync(categoria);
            return MapearRespuesta(creado);
        }

        public async Task<CategoriaResponseDTO?> ActualizarAsync(int id, CategoriaUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static CategoriaResponseDTO MapearRespuesta(Categoria c) => new()
        {
            Id = c.Id,
            NombreCategoria = c.Nombre,
            Descripcion = c.Descripcion,
            AreaId = c.AreaId,
            TipoTicketId = c.TipoTicketId,
            Estado = c.Estado
        };
    }
}