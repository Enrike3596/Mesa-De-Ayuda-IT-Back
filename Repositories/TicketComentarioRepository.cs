using Data;
using DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITicketComentarioRepository
    {
        Task<List<TicketComentario>> ObtenerTodosAsync();
        Task<TicketComentario?> ObtenerPorIdAsync(int id);
        Task<TicketComentario> CrearAsync(TicketComentario TicketComentario);
        Task<TicketComentario?> ActualizarAsync(int id, TicketComentarioUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<List<TicketComentario>> ObtenerPorTicketAsync(int ticketId);
    }

    public class TicketComentarioRepository : ITicketComentarioRepository
    {
        private readonly DbContextcs _context;

        public TicketComentarioRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TicketComentario>> ObtenerTodosAsync()
        {
            return await _context.TicketComentarios.ToListAsync();
        }

        public async Task<TicketComentario?> ObtenerPorIdAsync(int id)
        {
            return await _context.TicketComentarios.FindAsync(id);
        }

        public async Task<List<TicketComentario>> ObtenerPorTicketAsync(int ticketId)
        {
            return await _context.TicketComentarios
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<TicketComentario> CrearAsync(TicketComentario TicketComentario)
        {
            TicketComentario.Comentario = (TicketComentario.Comentario ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(TicketComentario.Comentario))
                throw new ArgumentException("Comentario no puede ser vacío.", nameof(TicketComentario));

            if (TicketComentario.TicketId <= 0)
                throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(TicketComentario));
            if (TicketComentario.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(TicketComentario));

            if (TicketComentario.FechaCreacion == default)
                TicketComentario.FechaCreacion = DateTime.UtcNow;

            _context.TicketComentarios.Add(TicketComentario);
            await _context.SaveChangesAsync();
            return TicketComentario;
        }

        public async Task<TicketComentario?> ActualizarAsync(int id, TicketComentarioUpdateDTO dto)
        {
            var TicketComentario = await _context.TicketComentarios.FindAsync(id);
            if (TicketComentario == null) return null;

            if (dto.Comentario != null)
            {
                var comentario = dto.Comentario.Trim();
                if (string.IsNullOrWhiteSpace(comentario))
                    throw new ArgumentException("Comentario no puede ser vacío.", nameof(dto.Comentario));
                TicketComentario.Comentario = comentario;
            }

            if (dto.UsuarioId.HasValue)
            {
                if (dto.UsuarioId.Value <= 0)
                    throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(dto.UsuarioId));
                TicketComentario.UsuarioId = dto.UsuarioId.Value;
            }

            if (dto.TicketId.HasValue)
            {
                if (dto.TicketId.Value <= 0)
                    throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(dto.TicketId));
                TicketComentario.TicketId = dto.TicketId.Value;
            }

            if (dto.EsInterno.HasValue)
            {
                TicketComentario.EsInterno = dto.EsInterno.Value;
            }

            await _context.SaveChangesAsync();
            return TicketComentario;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var TicketComentario = await _context.TicketComentarios.FindAsync(id);
            if (TicketComentario == null) return false;

            _context.TicketComentarios.Remove(TicketComentario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}