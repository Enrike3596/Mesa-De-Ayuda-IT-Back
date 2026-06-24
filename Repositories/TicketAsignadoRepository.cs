using Data;
using DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITicketAsignadoRepository
    {
        Task<List<TicketAsignado>> ObtenerTodosAsync();
        Task<TicketAsignado?> ObtenerPorIdAsync(int id);
        Task<TicketAsignado> CrearAsync(TicketAsignado TicketAsignado);
        Task<TicketAsignado?> ActualizarAsync(int id, TicketAsignadoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TicketAsignadoRepository : ITicketAsignadoRepository
    {
        private readonly DbContextcs _context;

        public TicketAsignadoRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TicketAsignado>> ObtenerTodosAsync()
        {
            return await _context.TicketAsignados.ToListAsync();
        }

        public async Task<TicketAsignado?> ObtenerPorIdAsync(int id)
        {
            return await _context.TicketAsignados.FindAsync(id);
        }

        public async Task<TicketAsignado> CrearAsync(TicketAsignado TicketAsignado)
        {
            if (TicketAsignado.TicketId <= 0)
                throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(TicketAsignado));
            if (TicketAsignado.UsuarioAgenteId <= 0)
                throw new ArgumentException("UsuarioAgenteId debe ser mayor que 0.", nameof(TicketAsignado));

            if (TicketAsignado.FechaAsignacion == default)
                TicketAsignado.FechaAsignacion = DateTime.UtcNow;

            _context.TicketAsignados.Add(TicketAsignado);
            await _context.SaveChangesAsync();
            return TicketAsignado;
        }

        public async Task<TicketAsignado?> ActualizarAsync(int id, TicketAsignadoUpdateDTO dto)
        {
            var TicketAsignado = await _context.TicketAsignados.FindAsync(id);
            if (TicketAsignado == null) return null;

            if (dto.TicketId.HasValue)
            {
                if (dto.TicketId.Value <= 0)
                    throw new ArgumentException("TicketId debe ser mayor que 0.", nameof(dto.TicketId));
                TicketAsignado.TicketId = dto.TicketId.Value;
            }

            if (dto.UsuarioId.HasValue)
            {
                if (dto.UsuarioId.Value <= 0)
                    throw new ArgumentException("UsuarioId debe ser mayor que 0.", nameof(dto.UsuarioId));
                TicketAsignado.UsuarioAgenteId = dto.UsuarioId.Value;
            }
            await _context.SaveChangesAsync();
            return TicketAsignado;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var TicketAsignado = await _context.TicketAsignados.FindAsync(id);
            if (TicketAsignado == null) return false;

            _context.TicketAsignados.Remove(TicketAsignado);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}