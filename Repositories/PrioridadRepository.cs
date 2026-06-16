using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IPrioridadRepository
    {
        Task<List<Prioridad>> ObtenerTodosAsync();
        Task<Prioridad?> ObtenerPorIdAsync(int id);
        Task<Prioridad> CrearAsync(Prioridad prioridad);
        Task<Prioridad?> ActualizarAsync(int id, PrioridadUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class PrioridadRepository : IPrioridadRepository
    {
        private readonly DbContextcs _context;

        public PrioridadRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Prioridad>> ObtenerTodosAsync()
        {
            return await _context.Prioridades
                .Where(p => p.Estado)
                .ToListAsync();
        }

        public async Task<Prioridad?> ObtenerPorIdAsync(int id)
        {
            return await _context.Prioridades.FindAsync(id);
        }

        public async Task<Prioridad> CrearAsync(Prioridad prioridad)
        {
            prioridad.Nombre = (prioridad.Nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(prioridad.Nombre))
                throw new ArgumentException("Nombre de prioridad no puede ser vacío.", nameof(prioridad));

            prioridad.Tipo = (prioridad.Tipo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(prioridad.Tipo))
                throw new ArgumentException("Tipo no puede ser vacío.", nameof(prioridad));

            if (prioridad.Hora_sla < 0)
                throw new ArgumentException("Hora_sla debe ser >= 0.", nameof(prioridad));

            _context.Prioridades.Add(prioridad);
            await _context.SaveChangesAsync();
            return prioridad;
        }

        public async Task<Prioridad?> ActualizarAsync(int id, PrioridadUpdateDTO dto)
        {
            var prioridad = await _context.Prioridades.FindAsync(id);
            if (prioridad == null) return null;

            if (dto.NombrePrioridad != null)
            {
                var nombre = dto.NombrePrioridad.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("Nombre de prioridad no puede ser vacío.", nameof(dto.NombrePrioridad));
                prioridad.Nombre = nombre;
            }

            if (dto.Tipo != null)
            {
                var tipo = dto.Tipo.Trim();
                if (string.IsNullOrWhiteSpace(tipo))
                    throw new ArgumentException("Tipo no puede ser vacío.", nameof(dto.Tipo));
                prioridad.Tipo = tipo;
            }

            if (dto.Hora_sla.HasValue)
            {
                if (dto.Hora_sla.Value < 0)
                    throw new ArgumentException("Hora_sla debe ser >= 0.", nameof(dto.Hora_sla));
                prioridad.Hora_sla = dto.Hora_sla.Value;
            }

            if (dto.Estado.HasValue) prioridad.Estado = dto.Estado.Value;

            await _context.SaveChangesAsync();
            return prioridad;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var prioridad = await _context.Prioridades.FindAsync(id);
            if (prioridad == null) return false;

            _context.Prioridades.Remove(prioridad);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}