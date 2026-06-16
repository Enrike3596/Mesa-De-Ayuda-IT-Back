using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IRolRepository
    {
        Task<List<Rol>> ObtenerTodosAsync();
        Task<Rol?> ObtenerPorIdAsync(int id);
        Task<Rol> CrearAsync(Rol rol);
        Task<Rol?> ActualizarAsync(int id, RolUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class RolRepository : IRolRepository
    {
        private readonly DbContextcs _context;

        public RolRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Rol>> ObtenerTodosAsync() =>
            await _context.Roles.ToListAsync();

        public async Task<Rol?> ObtenerPorIdAsync(int id) =>
            await _context.Roles.FindAsync(id);

        public async Task<Rol> CrearAsync(Rol rol)
        {
            rol.Nombre = (rol.Nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(rol.Nombre))
                throw new ArgumentException("Nombre de rol no puede ser vacío.", nameof(rol));

            rol.Tipo = (rol.Tipo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(rol.Tipo))
                throw new ArgumentException("Tipo de rol no puede ser vacío.", nameof(rol));

            rol.Estado = rol.Estado; // El modelo Rol usa bool, no requiere normalización de string

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<Rol?> ActualizarAsync(int id, RolUpdateDTO dto)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return null;

            if (dto.NombreRol != null)
            {
                var nombre = dto.NombreRol.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("Nombre de rol no puede ser vacío.", nameof(dto.NombreRol));
                rol.Nombre = nombre;
            }

            if (dto.Tipo != null)
            {
                var tipo = dto.Tipo.Trim();
                if (string.IsNullOrWhiteSpace(tipo))
                    throw new ArgumentException("Tipo de rol no puede ser vacío.", nameof(dto.Tipo));
                rol.Tipo = tipo;
            }

            if (dto.Estado != null)
                rol.Estado = dto.Estado.Value;

            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return false;

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}