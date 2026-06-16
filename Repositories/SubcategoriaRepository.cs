using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ISubcategoriaRepository
    {
        Task<List<Subcategoria>> ObtenerTodosAsync();
        Task<Subcategoria?> ObtenerPorIdAsync(int id);
        Task<Subcategoria> CrearAsync(Subcategoria subcategoria);
        Task<Subcategoria?> ActualizarAsync(int id, SubcategoriaUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class SubcategoriaRepository : ISubcategoriaRepository
    {
        private readonly DbContextcs _context;

        public SubcategoriaRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Subcategoria>> ObtenerTodosAsync()
        {
            return await _context.Subcategorias
                .Where(s => s.Estado)
                .ToListAsync();
        }

        public async Task<Subcategoria?> ObtenerPorIdAsync(int id)
        {
            return await _context.Subcategorias.FindAsync(id);
        }

        public async Task<Subcategoria> CrearAsync(Subcategoria subcategoria)
        {
            subcategoria.NombreSubcategoria = (subcategoria.NombreSubcategoria ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(subcategoria.NombreSubcategoria))
                throw new ArgumentException("NombreSubcategoria no puede ser vacío.", nameof(subcategoria));

            subcategoria.Descripcion = (subcategoria.Descripcion ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(subcategoria.Descripcion))
                throw new ArgumentException("Descripcion no puede ser vacía.", nameof(subcategoria));

            if (subcategoria.CategoriaId <= 0)
                throw new ArgumentException("CategoriaId debe ser mayor que 0.", nameof(subcategoria));

                subcategoria.Estado = subcategoria.Estado; // El modelo Subcategoria usa bool, no requiere normalización de string

            _context.Subcategorias.Add(subcategoria);
            await _context.SaveChangesAsync();
            return subcategoria;
        }

        public async Task<Subcategoria?> ActualizarAsync(int id, SubcategoriaUpdateDTO dto)
        {
            var subcategoria = await _context.Subcategorias.FindAsync(id);
            if (subcategoria == null) return null;

            if (dto.NombreSubcategoria != null)
            {
                var nombre = dto.NombreSubcategoria.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("NombreSubcategoria no puede ser vacío.", nameof(dto.NombreSubcategoria));
                subcategoria.NombreSubcategoria = nombre;
            }

            if (dto.Descripcion != null)
            {
                var descripcion = dto.Descripcion.Trim();
                if (string.IsNullOrWhiteSpace(descripcion))
                    throw new ArgumentException("Descripcion no puede ser vacía.", nameof(dto.Descripcion));
                subcategoria.Descripcion = descripcion;
            }

            if (dto.CategoriaId.HasValue)
            {
                if (dto.CategoriaId.Value <= 0)
                    throw new ArgumentException("CategoriaId debe ser mayor que 0.", nameof(dto.CategoriaId));
                subcategoria.CategoriaId = dto.CategoriaId.Value;
            }

            if (dto.Estado != null)
                subcategoria.Estado = dto.Estado.Value;

            await _context.SaveChangesAsync();
            return subcategoria;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var subcategoria = await _context.Subcategorias.FindAsync(id);
            if (subcategoria == null) return false;

            subcategoria.Estado = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}