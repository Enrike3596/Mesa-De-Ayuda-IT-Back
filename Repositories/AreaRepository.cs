using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IAreaRepository
    {
        Task<List<Area>> ObtenerTodosAsync();
        Task<Area?> ObtenerPorIdAsync(int id);
        Task<Area> CrearAsync(Area area);
        Task<Area?> ActualizarAsync(int id, AreaUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class AreaRepository : IAreaRepository
    {
        private readonly DbContextcs _context;

        public AreaRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Area>> ObtenerTodosAsync()
        {
            return await _context.Areas
            .Where(a => a.Estado)
            .ToListAsync();
        }

        public async Task<Area?> ObtenerPorIdAsync(int id)
        {
            return await _context.Areas.FindAsync(id);
        }

        public async Task<Area> CrearAsync(Area area)
        {
            area.NombreArea = (area.NombreArea ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(area.NombreArea))
                throw new ArgumentException("NombreArea no puede ser vacío.", nameof(area));

            var existeArea = await _context.Areas
                .AnyAsync(a => a.NombreArea.ToLower() == area.NombreArea.ToLower());

            if (existeArea)
                throw new InvalidOperationException($"Ya existe un área con el nombre '{area.NombreArea}'.");

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Area?> ActualizarAsync(int id, AreaUpdateDTO dto)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return null;

            if (dto.NombreArea != null)
            {
                var nombre = dto.NombreArea.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("NombreArea no puede ser vacío.", nameof(dto.NombreArea));

                var existeArea = await _context.Areas
                    .AnyAsync(a => a.Id != id && a.NombreArea.ToLower() == nombre.ToLower());

                if (existeArea)
                    throw new InvalidOperationException($"Ya existe un área con el nombre '{nombre}'.");

                area.NombreArea = nombre;
            }
            if (dto.Estado.HasValue) area.Estado = dto.Estado.Value;

            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return false;

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
