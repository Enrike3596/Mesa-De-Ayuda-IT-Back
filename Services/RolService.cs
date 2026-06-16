using DTOs;
using Helpers;
using Models;
using Repositories;

namespace Services
{
    public interface IRolService
    {
        Task<List<RolResponseDTO>> ObtenerTodosAsync();
        Task<RolResponseDTO?> ObtenerPorIdAsync(int id);
        Task<RolResponseDTO> CrearAsync(RolCreateDTO dto);
        Task<RolResponseDTO?> ActualizarAsync(int id, RolUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class RolService : IRolService
    {
        private readonly IRolRepository _repo;

        public RolService(IRolRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<RolResponseDTO>> ObtenerTodosAsync()
        {
            var roles = await _repo.ObtenerTodosAsync();
            return roles.Select(MapearRespuesta).ToList();
        }

        public async Task<RolResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var rol = await _repo.ObtenerPorIdAsync(id);
            return rol == null ? null : MapearRespuesta(rol);
        }

        public async Task<RolResponseDTO> CrearAsync(RolCreateDTO dto)
        {
            var rol = new Rol
            {
                Nombre = dto.NombreRol,
                Tipo = dto.Tipo,
                Estado = dto.Estado
            };

            var creado = await _repo.CrearAsync(rol);
            return MapearRespuesta(creado);
        }

        public async Task<RolResponseDTO?> ActualizarAsync(int id, RolUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static RolResponseDTO MapearRespuesta(Rol r) => new()
        {
            Id = r.Id,
            NombreRol = r.Nombre,
            Tipo = r.Tipo,
            Estado = r.Estado
        };
    }
}