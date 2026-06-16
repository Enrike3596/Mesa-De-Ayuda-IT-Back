using DTOs;
using Models;
using Repositories;

namespace Services
{
    public interface ISubcategoriaService
    {
        Task<List<SubcategoriaResponseDTO>> ObtenerTodosAsync();
        Task<SubcategoriaResponseDTO?> ObtenerPorIdAsync(int id);
        Task<SubcategoriaResponseDTO> CrearAsync(SubcategoriaCreateDTO dto);
        Task<SubcategoriaResponseDTO?> ActualizarAsync(int id, SubcategoriaUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }   

    public class SubcategoriaService : ISubcategoriaService
    {
        private readonly ISubcategoriaRepository _repo;

        public SubcategoriaService(ISubcategoriaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SubcategoriaResponseDTO>> ObtenerTodosAsync()
        {
            var subcategorias = await _repo.ObtenerTodosAsync();
            return subcategorias.Select(MapearRespuesta).ToList();
        }

        public async Task<SubcategoriaResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var subcategoria = await _repo.ObtenerPorIdAsync(id);
            return subcategoria == null ? null : MapearRespuesta(subcategoria);
        }

        public async Task<SubcategoriaResponseDTO> CrearAsync(SubcategoriaCreateDTO dto)
        {
            var subcategoria = new Subcategoria
            {
                NombreSubcategoria = dto.NombreSubcategoria,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                Estado = dto.Estado
            };

            var creado = await _repo.CrearAsync(subcategoria);
            return MapearRespuesta(creado);
        }

        public async Task<SubcategoriaResponseDTO?> ActualizarAsync(int id, SubcategoriaUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static SubcategoriaResponseDTO MapearRespuesta(Subcategoria s) => new()
        {
            Id = s.Id,
            NombreSubcategoria = s.NombreSubcategoria,
            CategoriaId = s.CategoriaId,
            Descripcion = s.Descripcion,
            Estado = s.Estado
        };
    }
}