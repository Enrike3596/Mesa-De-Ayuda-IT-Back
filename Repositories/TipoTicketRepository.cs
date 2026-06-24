using Data;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITipoTicketRepository
    {
        Task<List<TipoTicket>> ObtenerTodosAsync();
        Task<TipoTicket?> ObtenerPorIdAsync(int id);
    }

    public class TipoTicketRepository : ITipoTicketRepository
    {
        private readonly DbContextcs _context;

        public TipoTicketRepository(DbContextcs context)
        {
            _context = context;
        }

        public async Task<List<TipoTicket>> ObtenerTodosAsync()
        {
            return await _context.TipoTickets.ToListAsync();
        }

        public async Task<TipoTicket?> ObtenerPorIdAsync(int id)
        {
            return await _context.TipoTickets.FindAsync(id);
        }
    }
}
