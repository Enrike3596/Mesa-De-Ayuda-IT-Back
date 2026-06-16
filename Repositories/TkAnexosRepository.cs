using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITkAnexosRepository
    {
        Task<List<TkAnexo>> ObtenerTodosAsync();
        Task<List<TkAnexo>> ObtenerPorTicketIdAsync(int ticketId);
        Task<TkAnexo?> ObtenerPorIdAsync(int id);
        Task<TkAnexo> CrearAsync(TkAnexo tkAnexo);
        Task<TkAnexo?> ActualizarAsync(int id, TkAnexoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TkAnexosRepository : ITkAnexosRepository
    {
        private readonly DbContextcs _context;

        public TkAnexosRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TkAnexo>> ObtenerTodosAsync()
        {
            return await _context.TkAnexos
                .Where(a => a.Estado != null && a.Estado.ToLower() == "activo")
                .ToListAsync();
        }

        public async Task<List<TkAnexo>> ObtenerPorTicketIdAsync(int ticketId)
        {
            if (ticketId <= 0) return new List<TkAnexo>();

            return await _context.TkAnexos
                .Where(a => a.TicketId == ticketId)
                .Where(a => a.Estado != null && a.Estado.ToLower() == "activo")
                .ToListAsync();
        }

        public async Task<TkAnexo?> ObtenerPorIdAsync(int id)
        {
            return await _context.TkAnexos.FindAsync(id);
        }

        public async Task<TkAnexo> CrearAsync(TkAnexo tkAnexo)
        {
            tkAnexo.NombreArchivo = (tkAnexo.NombreArchivo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tkAnexo.NombreArchivo))
                throw new ArgumentException("NombreArchivo no puede ser vacío.", nameof(tkAnexo));

            tkAnexo.TipoArchivo = (tkAnexo.TipoArchivo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tkAnexo.TipoArchivo))
                throw new ArgumentException("TipoArchivo no puede ser vacío.", nameof(tkAnexo));

            tkAnexo.UrlArchivo = (tkAnexo.UrlArchivo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tkAnexo.UrlArchivo))
                throw new ArgumentException("UrlArchivo no puede ser vacía.", nameof(tkAnexo));

            if (tkAnexo.TamanoArchivo < 0)
                throw new ArgumentException("TamanoArchivo debe ser >= 0.", nameof(tkAnexo));

            if (tkAnexo.TicketId <= 0)
                throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(tkAnexo));

            var ticketExiste = await _context.Tickets.AnyAsync(t => t.Id == tkAnexo.TicketId);
            if (!ticketExiste)
                throw new ArgumentException($"El ticket con ID {tkAnexo.TicketId} no existe.", nameof(tkAnexo.TicketId));

            if (tkAnexo.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(tkAnexo));

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == tkAnexo.UsuarioId);
            if (!usuarioExiste)
                throw new ArgumentException($"El usuario con ID {tkAnexo.UsuarioId} no existe.", nameof(tkAnexo.UsuarioId));

            tkAnexo.Estado = string.IsNullOrWhiteSpace(tkAnexo.Estado)
                ? EstadoNormalizer.Activo
                : EstadoNormalizer.Normalize(tkAnexo.Estado);

            if (tkAnexo.FechaCarga == default)
                tkAnexo.FechaCarga = DateTime.UtcNow;

            _context.TkAnexos.Add(tkAnexo);
            await _context.SaveChangesAsync();
            return tkAnexo;
        }

        public async Task<TkAnexo?> ActualizarAsync(int id, TkAnexoUpdateDTO dto)
        {
            var tkAnexo = await _context.TkAnexos.FindAsync(id);
            if (tkAnexo == null) return null;

            if (dto.NombreArchivo != null)
            {
                var nombre = dto.NombreArchivo.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("NombreArchivo no puede ser vacío.", nameof(dto.NombreArchivo));
                tkAnexo.NombreArchivo = nombre;
            }

            if (dto.TipoArchivo != null)
            {
                var tipo = dto.TipoArchivo.Trim();
                if (string.IsNullOrWhiteSpace(tipo))
                    throw new ArgumentException("TipoArchivo no puede ser vacío.", nameof(dto.TipoArchivo));
                tkAnexo.TipoArchivo = tipo;
            }

            if (dto.UrlArchivo != null)
            {
                var url = dto.UrlArchivo.Trim();
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentException("UrlArchivo no puede ser vacía.", nameof(dto.UrlArchivo));
                tkAnexo.UrlArchivo = url;
            }

            if (dto.TamanoArchivo.HasValue)
            {
                if (dto.TamanoArchivo.Value < 0)
                    throw new ArgumentException("TamanoArchivo debe ser >= 0.", nameof(dto.TamanoArchivo));
                tkAnexo.TamanoArchivo = dto.TamanoArchivo.Value;
            }

            if (dto.TicketId.HasValue)
            {
                if (dto.TicketId.Value <= 0)
                    throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(dto.TicketId));
                tkAnexo.TicketId = dto.TicketId.Value;
            }

            if (dto.UsuarioId.HasValue)
            {
                if (dto.UsuarioId.Value <= 0)
                    throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(dto.UsuarioId));
                tkAnexo.UsuarioId = dto.UsuarioId.Value;
            }

            if (dto.Estado != null)
                tkAnexo.Estado = EstadoNormalizer.Normalize(dto.Estado);

            await _context.SaveChangesAsync();
            return tkAnexo;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var tkAnexo = await _context.TkAnexos.FindAsync(id);
            if (tkAnexo == null) return false;

            tkAnexo.Estado = EstadoNormalizer.Inactivo;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
