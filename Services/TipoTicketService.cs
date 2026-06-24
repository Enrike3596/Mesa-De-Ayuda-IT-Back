using DTOs;
using Repositories;

namespace Services
{
    public interface ITipoTicketService
    {
        Task<List<TipoTicketInfo>> ObtenerTodosAsync();
        Task<TipoTicketInfo?> ObtenerPorIdAsync(int id);
    }

    public class TipoTicketService : ITipoTicketService
    {
        private readonly ITipoTicketRepository _repo;

        public TipoTicketService(ITipoTicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TipoTicketInfo>> ObtenerTodosAsync()
        {
            var tipos = await _repo.ObtenerTodosAsync();
            return tipos.Select(t => new TipoTicketInfo
            {
                Id = t.Id,
                Nombre = t.Nombre
            }).ToList();
        }

        public async Task<TipoTicketInfo?> ObtenerPorIdAsync(int id)
        {
            var tipo = await _repo.ObtenerPorIdAsync(id);
            return tipo == null ? null : new TipoTicketInfo
            {
                Id = tipo.Id,
                Nombre = tipo.Nombre
            };
        }
    }
}
