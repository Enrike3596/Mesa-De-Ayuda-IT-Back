using DTOs;
using Helpers;
using Models;
using Repositories;

namespace Services
{
    public interface IUsuarioService
    {
        Task<List<UsuarioResponseDTO>> ObtenerTodosAsync();
        Task<UsuarioResponseDTO?> ObtenerPorIdAsync(int id);
        Task<UsuarioResponseDTO> CrearAsync(UsuarioCreateDTO dto);
        Task<UsuarioResponseDTO?> ActualizarAsync(int id, UsuarioUpdateDTO dto);
        Task<List<UsuarioResponseDTO>> ObtenerPorAreaAsync(int areaId);
        Task<bool> EliminarAsync(int id);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UsuarioResponseDTO>> ObtenerTodosAsync()
        {
            var usuarios = await _repo.ObtenerTodosAsync();
            return usuarios.Select(MapearRespuesta).ToList();
        }

        public async Task<UsuarioResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var usuario = await _repo.ObtenerPorIdAsync(id);
            return usuario == null ? null : MapearRespuesta(usuario);
        }

        public async Task<UsuarioResponseDTO> CrearAsync(UsuarioCreateDTO dto)
        {
            var contrasenaPlano = string.IsNullOrWhiteSpace(dto.ContrasenaHash)
                ? dto.Contrasena
                : dto.ContrasenaHash;

            if (string.IsNullOrWhiteSpace(contrasenaPlano))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto));

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Estado = dto.Estado.HasValue ? dto.Estado.Value : true,
                RolId = dto.RolId,
                AreaId = dto.AreaId,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasenaPlano)
            };

            var creado = await _repo.CrearAsync(usuario);
            return MapearRespuesta(creado);
        }

        public async Task<UsuarioResponseDTO?> ActualizarAsync(int id, UsuarioUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public async Task<List<UsuarioResponseDTO>> ObtenerPorAreaAsync(int areaId)
        {
            var usuarios = await _repo.ObtenerPorAreaIdAsync(areaId);
            return usuarios.Select(MapearRespuesta).ToList();
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _repo.EliminarAsync(id);
        }

        // Mapeo interno: Model → ResponseDTO
        private static UsuarioResponseDTO MapearRespuesta(Usuario u) => new()
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Correo = u.Correo,
            Telefono = u.Telefono,
            Estado = u.Estado,
            RolId = u.RolId,
            Rol = u.Rol?.Nombre ?? "Sin rol",
            AreaId = u.AreaId,
            Area = u.Area?.NombreArea ?? "Sin área",
            FechaCreacion = u.FechaCreacion
        };
    }
}
