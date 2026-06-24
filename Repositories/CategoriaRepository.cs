using Data;
using DTOs;
using Helpers;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> ObtenerTodosAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task<Categoria> CrearAsync(Categoria categoria);
        Task<Categoria?> ActualizarAsync(int id, CategoriaUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly DbContextcs _context;

        public CategoriaRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> ObtenerTodosAsync()
        {
            return await _context.Categorias
                .Where(c => c.Estado)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        public async Task<Categoria> CrearAsync(Categoria categoria)
        {
            categoria.Nombre = (categoria.Nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                throw new ArgumentException("Nombre de categoría no puede ser vacío.", nameof(categoria));

            categoria.Nombre = (categoria.Nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                throw new ArgumentException("Nombre de categoría no puede ser vacío.", nameof(categoria));

            categoria.Descripcion = (categoria.Descripcion ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                throw new ArgumentException("Descripción no puede ser vacía.", nameof(categoria));

            if (categoria.AreaId <= 0)
                throw new ArgumentException("AreaId debe ser mayor que 0.", nameof(categoria));

            if (categoria.TipoTicketId <= 0)
                throw new ArgumentException("TipoTicketId debe ser mayor que 0.", nameof(categoria));

            categoria.Estado = categoria.Estado; // El modelo Categoria usa bool, no requiere normalización de string

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria?> ActualizarAsync(int id, CategoriaUpdateDTO dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return null;

            if (dto.NombreCategoria != null)
            {
                var nombre = dto.NombreCategoria.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("Nombre de categoría no puede ser vacío.", nameof(dto.NombreCategoria));
                categoria.Nombre = nombre;
            }

            if (dto.Descripcion != null)
            {
                var descripcion = dto.Descripcion.Trim();
                if (string.IsNullOrWhiteSpace(descripcion))
                    throw new ArgumentException("Descripción no puede ser vacía.", nameof(dto.Descripcion));
                categoria.Descripcion = descripcion;
            }

            if (dto.AreaId.HasValue)
            {
                if (dto.AreaId.Value <= 0)
                    throw new ArgumentException("AreaId debe ser mayor que 0.", nameof(dto.AreaId));
                categoria.AreaId = dto.AreaId.Value;
            }

            if (dto.TipoTicketId.HasValue)
            {
                if (dto.TipoTicketId.Value <= 0)
                    throw new ArgumentException("TipoTicketId debe ser mayor que 0.", nameof(dto.TipoTicketId));
                categoria.TipoTicketId = dto.TipoTicketId.Value;
            }

            if (dto.Estado != null) categoria.Estado = dto.Estado.Value;

            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            categoria.Estado = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}