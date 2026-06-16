using Data;
using DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITkAsignadoRepository
    {
        Task<List<TkAsignado>> ObtenerTodosAsync();
        Task<TkAsignado?> ObtenerPorIdAsync(int id);
        Task<TkAsignado> CrearAsync(TkAsignado tkAsignado);
        Task<TkAsignado?> ActualizarAsync(int id, TkAsignadoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TkAsignadoRepository : ITkAsignadoRepository
    {
        private readonly DbContextcs _context;

        public TkAsignadoRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TkAsignado>> ObtenerTodosAsync()
        {
            return await _context.TkAsignados.ToListAsync();
        }

        public async Task<TkAsignado?> ObtenerPorIdAsync(int id)
        {
            return await _context.TkAsignados.FindAsync(id);
        }

        public async Task<TkAsignado> CrearAsync(TkAsignado tkAsignado)
        {
            if (tkAsignado.TicketId <= 0)
                throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(tkAsignado));
            if (tkAsignado.UsuarioAgenteId <= 0)
                throw new ArgumentException("UsuarioAgenteId debe ser mayor que 0.", nameof(tkAsignado));

            if (tkAsignado.FechaAsignacion == default)
                tkAsignado.FechaAsignacion = DateTime.UtcNow;

            _context.TkAsignados.Add(tkAsignado);
            await _context.SaveChangesAsync();
            return tkAsignado;
        }

        public async Task<TkAsignado?> ActualizarAsync(int id, TkAsignadoUpdateDTO dto)
        {
            var tkAsignado = await _context.TkAsignados.FindAsync(id);
            if (tkAsignado == null) return null;

            if (dto.TicketId.HasValue)
            {
                if (dto.TicketId.Value <= 0)
                    throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(dto.TicketId));
                tkAsignado.TicketId = dto.TicketId.Value;
            }

            if (dto.UsuarioId.HasValue)
            {
                if (dto.UsuarioId.Value <= 0)
                    throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(dto.UsuarioId));
                tkAsignado.UsuarioAgenteId = dto.UsuarioId.Value;
            }
            await _context.SaveChangesAsync();
            return tkAsignado;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var tkAsignado = await _context.TkAsignados.FindAsync(id);
            if (tkAsignado == null) return false;

            _context.TkAsignados.Remove(tkAsignado);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}