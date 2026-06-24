using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITicketAnexosRepository
    {
        Task<List<TicketAnexo>> ObtenerTodosAsync();
        Task<List<TicketAnexo>> ObtenerPorTicketIdAsync(int ticketId);
        Task<TicketAnexo?> ObtenerPorIdAsync(int id);
        Task<TicketAnexo> CrearAsync(TicketAnexo TicketAnexo);
        Task<TicketAnexo?> ActualizarAsync(int id, TicketAnexoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TicketAnexosRepository : ITicketAnexosRepository
    {
        private readonly DbContextcs _context;

        public TicketAnexosRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TicketAnexo>> ObtenerTodosAsync()
        {
            return await _context.TicketAnexos
                .Where(a => a.Estado != null && a.Estado.ToLower() == "activo")
                .ToListAsync();
        }

        public async Task<List<TicketAnexo>> ObtenerPorTicketIdAsync(int ticketId)
        {
            if (ticketId <= 0) return new List<TicketAnexo>();

            return await _context.TicketAnexos
                .Where(a => a.TicketId == ticketId)
                .Where(a => a.Estado != null && a.Estado.ToLower() == "activo")
                .ToListAsync();
        }

        public async Task<TicketAnexo?> ObtenerPorIdAsync(int id)
        {
            return await _context.TicketAnexos.FindAsync(id);
        }

        public async Task<TicketAnexo> CrearAsync(TicketAnexo TicketAnexo)
        {
            TicketAnexo.NombreArchivo = (TicketAnexo.NombreArchivo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(TicketAnexo.NombreArchivo))
                throw new ArgumentException("NombreArchivo no puede ser vacío.", nameof(TicketAnexo));

            TicketAnexo.TipoArchivo = (TicketAnexo.TipoArchivo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(TicketAnexo.TipoArchivo))
                throw new ArgumentException("TipoArchivo no puede ser vacío.", nameof(TicketAnexo));

            TicketAnexo.UrlArchivo = (TicketAnexo.UrlArchivo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(TicketAnexo.UrlArchivo))
                throw new ArgumentException("UrlArchivo no puede ser vacía.", nameof(TicketAnexo));

            if (TicketAnexo.TamanoArchivo < 0)
                throw new ArgumentException("TamanoArchivo debe ser >= 0.", nameof(TicketAnexo));

            if (TicketAnexo.TicketId <= 0)
                throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(TicketAnexo));

            var ticketExiste = await _context.Tickets.AnyAsync(t => t.Id == TicketAnexo.TicketId);
            if (!ticketExiste)
                throw new ArgumentException($"El ticket con ID {TicketAnexo.TicketId} no existe.", nameof(TicketAnexo.TicketId));

            if (TicketAnexo.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(TicketAnexo));

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == TicketAnexo.UsuarioId);
            if (!usuarioExiste)
                throw new ArgumentException($"El usuario con ID {TicketAnexo.UsuarioId} no existe.", nameof(TicketAnexo.UsuarioId));

            TicketAnexo.Estado = string.IsNullOrWhiteSpace(TicketAnexo.Estado)
                ? EstadoNormalizer.Activo
                : EstadoNormalizer.Normalize(TicketAnexo.Estado);

            if (TicketAnexo.FechaCarga == default)
                TicketAnexo.FechaCarga = DateTime.UtcNow;

            _context.TicketAnexos.Add(TicketAnexo);
            await _context.SaveChangesAsync();
            return TicketAnexo;
        }

        public async Task<TicketAnexo?> ActualizarAsync(int id, TicketAnexoUpdateDTO dto)
        {
            var TicketAnexo = await _context.TicketAnexos.FindAsync(id);
            if (TicketAnexo == null) return null;

            if (dto.NombreArchivo != null)
            {
                var nombre = dto.NombreArchivo.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("NombreArchivo no puede ser vacío.", nameof(dto.NombreArchivo));
                TicketAnexo.NombreArchivo = nombre;
            }

            if (dto.TipoArchivo != null)
            {
                var tipo = dto.TipoArchivo.Trim();
                if (string.IsNullOrWhiteSpace(tipo))
                    throw new ArgumentException("TipoArchivo no puede ser vacío.", nameof(dto.TipoArchivo));
                TicketAnexo.TipoArchivo = tipo;
            }

            if (dto.UrlArchivo != null)
            {
                var url = dto.UrlArchivo.Trim();
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentException("UrlArchivo no puede ser vacía.", nameof(dto.UrlArchivo));
                TicketAnexo.UrlArchivo = url;
            }

            if (dto.TamanoArchivo.HasValue)
            {
                if (dto.TamanoArchivo.Value < 0)
                    throw new ArgumentException("TamanoArchivo debe ser >= 0.", nameof(dto.TamanoArchivo));
                TicketAnexo.TamanoArchivo = dto.TamanoArchivo.Value;
            }

            if (dto.TicketId.HasValue)
            {
                if (dto.TicketId.Value <= 0)
                    throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(dto.TicketId));
                TicketAnexo.TicketId = dto.TicketId.Value;
            }

            if (dto.UsuarioId.HasValue)
            {
                if (dto.UsuarioId.Value <= 0)
                    throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(dto.UsuarioId));
                TicketAnexo.UsuarioId = dto.UsuarioId.Value;
            }

            if (dto.Estado != null)
                TicketAnexo.Estado = EstadoNormalizer.Normalize(dto.Estado);

            await _context.SaveChangesAsync();
            return TicketAnexo;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var TicketAnexo = await _context.TicketAnexos.FindAsync(id);
            if (TicketAnexo == null) return false;

            TicketAnexo.Estado = EstadoNormalizer.Inactivo;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
