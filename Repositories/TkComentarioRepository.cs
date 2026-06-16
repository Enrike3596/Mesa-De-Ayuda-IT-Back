using Data;
using DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITkComentarioRepository
    {
        Task<List<TkComentario>> ObtenerTodosAsync();
        Task<TkComentario?> ObtenerPorIdAsync(int id);
        Task<TkComentario> CrearAsync(TkComentario tkComentario);
        Task<TkComentario?> ActualizarAsync(int id, TkComentarioUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<List<TkComentario>> ObtenerPorTicketAsync(int ticketId);
    }

    public class TkComentarioRepository : ITkComentarioRepository
    {
        private readonly DbContextcs _context;

        public TkComentarioRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TkComentario>> ObtenerTodosAsync()
        {
            return await _context.TkComentarios.ToListAsync();
        }

        public async Task<TkComentario?> ObtenerPorIdAsync(int id)
        {
            return await _context.TkComentarios.FindAsync(id);
        }

        public async Task<List<TkComentario>> ObtenerPorTicketAsync(int ticketId)
        {
            return await _context.TkComentarios
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<TkComentario> CrearAsync(TkComentario tkComentario)
        {
            tkComentario.Comentario = (tkComentario.Comentario ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tkComentario.Comentario))
                throw new ArgumentException("Comentario no puede ser vacío.", nameof(tkComentario));

            if (tkComentario.TicketId <= 0)
                throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(tkComentario));
            if (tkComentario.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(tkComentario));

            if (tkComentario.FechaCreacion == default)
                tkComentario.FechaCreacion = DateTime.UtcNow;

            _context.TkComentarios.Add(tkComentario);
            await _context.SaveChangesAsync();
            return tkComentario;
        }

        public async Task<TkComentario?> ActualizarAsync(int id, TkComentarioUpdateDTO dto)
        {
            var tkComentario = await _context.TkComentarios.FindAsync(id);
            if (tkComentario == null) return null;

            if (dto.Comentario != null)
            {
                var comentario = dto.Comentario.Trim();
                if (string.IsNullOrWhiteSpace(comentario))
                    throw new ArgumentException("Comentario no puede ser vacío.", nameof(dto.Comentario));
                tkComentario.Comentario = comentario;
            }

            if (dto.UsuarioId.HasValue)
            {
                if (dto.UsuarioId.Value <= 0)
                    throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(dto.UsuarioId));
                tkComentario.UsuarioId = dto.UsuarioId.Value;
            }

            if (dto.TicketId.HasValue)
            {
                if (dto.TicketId.Value <= 0)
                    throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(dto.TicketId));
                tkComentario.TicketId = dto.TicketId.Value;
            }

            if (dto.EsInterno.HasValue)
            {
                tkComentario.EsInterno = dto.EsInterno.Value;
            }

            await _context.SaveChangesAsync();
            return tkComentario;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var tkComentario = await _context.TkComentarios.FindAsync(id);
            if (tkComentario == null) return false;

            _context.TkComentarios.Remove(tkComentario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}