using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Repositories
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorCorreoAsync(string correo);
        Task<Usuario> CrearAsync(Usuario usuario);
        Task<Usuario?> ActualizarAsync(int id, UsuarioUpdateDTO dto);
        Task<List<Usuario>> ObtenerPorAreaIdAsync(int areaId);
        Task<bool> EliminarAsync(int id);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbContextcs _context;

        public UsuarioRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Area)
                .Where(u => u.Estado)
                .ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            var normalizedCorreo = (correo ?? string.Empty).Trim().ToLowerInvariant();
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.Correo.ToLower() == normalizedCorreo);
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            usuario.Nombre = (usuario.Nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new ArgumentException("Nombre no puede ser vacío.", nameof(usuario));

            usuario.Correo = (usuario.Correo ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(usuario.Correo))
                throw new ArgumentException("Correo no puede ser vacío.", nameof(usuario));

            usuario.Telefono = (usuario.Telefono ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(usuario.Telefono))
                throw new ArgumentException("Teléfono no puede ser vacío.", nameof(usuario));

            if (usuario.FechaCreacion == default)
                usuario.FechaCreacion = DateTime.UtcNow;
            usuario.FechaModificacion = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);

            try
            {
                await _context.SaveChangesAsync();
                await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();
                await _context.Entry(usuario).Reference(u => u.Area).LoadAsync();
                return usuario;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg
                                              && pg.SqlState == PostgresErrorCodes.UniqueViolation
                                              && string.Equals(pg.ConstraintName, "PK_Usuarios", StringComparison.OrdinalIgnoreCase))
            {
                // Typical after seeding fixed IDs: the identity/sequence may not have been advanced.
                // Repair sequence and retry once.
                await FixUsuariosIdSequenceAsync();

                // Ensure entity is still marked as Added before retrying.
                _context.Entry(usuario).State = EntityState.Added;
                await _context.SaveChangesAsync();
                await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();
                await _context.Entry(usuario).Reference(u => u.Area).LoadAsync();
                return usuario;
            }
        }

        private Task FixUsuariosIdSequenceAsync()
        {
            // pg_get_serial_sequence needs the quoted table name to preserve case.
            return _context.Database.ExecuteSqlRawAsync(
                "SELECT setval(pg_get_serial_sequence('\"Usuarios\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Usuarios\"));");
        }

        public async Task<Usuario?> ActualizarAsync(int id, UsuarioUpdateDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return null;

            if (dto.Nombre != null)
            {
                var nombre = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("Nombre no puede ser vacío.", nameof(dto.Nombre));
                usuario.Nombre = nombre;
            }

            if (dto.Telefono != null)
            {
                var telefono = dto.Telefono.Trim();
                if (string.IsNullOrWhiteSpace(telefono))
                    throw new ArgumentException("Teléfono no puede ser vacío.", nameof(dto.Telefono));
                usuario.Telefono = telefono;
            }

            if (dto.RolId.HasValue)
                usuario.RolId = dto.RolId.Value;

            if (dto.AreaId.HasValue)
                usuario.AreaId = dto.AreaId.Value;

            if (dto.Estado.HasValue) usuario.Estado = dto.Estado.Value;

            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();
            await _context.Entry(usuario).Reference(u => u.Area).LoadAsync();
            return usuario;
        }

        public async Task<List<Usuario>> ObtenerPorAreaIdAsync(int areaId)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Area)
                .Where(u => u.AreaId == areaId && u.Estado)
                .ToListAsync();
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            // Baja lógica, no física

            usuario.Estado = false;
            usuario.FechaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
