using DTOs;
using Helpers;
using Microsoft.AspNetCore.Http;
using Models;
using Repositories;

namespace Services
{
    public interface ITkAnexoService
    {
        Task<List<TkAnexoResponseDTO>> ObtenerTodosAsync();
        Task<List<TkAnexoResponseDTO>> ObtenerPorTicketIdAsync(int ticketId);
        Task<TkAnexoResponseDTO?> ObtenerPorIdAsync(int id);
        Task<TkAnexoResponseDTO> CrearAsync(TkAnexoCreateDTO dto);
        Task<TkAnexoResponseDTO> SubirArchivoAsync(IFormFile archivo, int ticketId, int usuarioId);
        Task<TkAnexoResponseDTO?> ActualizarAsync(int id, TkAnexoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TkAnexoService : ITkAnexoService
    {
        private readonly ITkAnexosRepository _repo;
        private readonly IFileStorageService _fileStorage;
        private readonly ITicketRepository _ticketRepo;

        public TkAnexoService(ITkAnexosRepository repo, IFileStorageService fileStorage, ITicketRepository ticketRepo)
        {
            _repo = repo;
            _fileStorage = fileStorage;
            _ticketRepo = ticketRepo;
        }

        public async Task<List<TkAnexoResponseDTO>> ObtenerTodosAsync()
        {
            var anexos = await _repo.ObtenerTodosAsync();
            return anexos.Select(MapearRespuesta).ToList();
        }

        public async Task<List<TkAnexoResponseDTO>> ObtenerPorTicketIdAsync(int ticketId)
        {
            var anexos = await _repo.ObtenerPorTicketIdAsync(ticketId);
            return anexos.Select(MapearRespuesta).ToList();
        }

        public async Task<TkAnexoResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var anexo = await _repo.ObtenerPorIdAsync(id);
            return anexo == null ? null : MapearRespuesta(anexo);
        }

        public async Task<TkAnexoResponseDTO> CrearAsync(TkAnexoCreateDTO dto)
        {
            var anexo = new TkAnexo
            {
                TicketId = dto.TicketId,
                UsuarioId = dto.UsuarioId,
                NombreArchivo = dto.NombreArchivo,
                TipoArchivo = dto.TipoArchivo,
                TamanoArchivo = dto.TamanoArchivo,
                UrlArchivo = dto.UrlArchivo,
                Estado = dto.Estado ?? string.Empty
            };

            var creado = await _repo.CrearAsync(anexo);
            return MapearRespuesta(creado);
        }

        public async Task<TkAnexoResponseDTO> SubirArchivoAsync(IFormFile archivo, int ticketId, int usuarioId)
        {
            var ticket = await _ticketRepo.ObtenerPorIdAsync(ticketId);
            if (ticket == null)
                throw new ArgumentException($"El ticket con ID {ticketId} no existe.", nameof(ticketId));

            if (TicketEstadoNormalizer.Normalize(ticket.Estado) == TicketEstadoNormalizer.Cerrado)
                throw new InvalidOperationException("No se pueden adjuntar archivos a un ticket cerrado.");

            var url = await _fileStorage.SaveFileAsync(archivo, ticketId);

            var anexo = new TkAnexo
            {
                TicketId = ticketId,
                UsuarioId = usuarioId,
                NombreArchivo = archivo.FileName,
                TipoArchivo = archivo.ContentType,
                TamanoArchivo = archivo.Length,
                UrlArchivo = url,
                Estado = "Activo"
            };

            var creado = await _repo.CrearAsync(anexo);
            return MapearRespuesta(creado);
        }

        public async Task<TkAnexoResponseDTO?> ActualizarAsync(int id, TkAnexoUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static TkAnexoResponseDTO MapearRespuesta(TkAnexo a) => new()
        {
            Id = a.Id,
            NombreArchivo = a.NombreArchivo,
            TicketId = a.TicketId,
            UsuarioId = a.UsuarioId,
            UrlArchivo = a.UrlArchivo,
            TipoArchivo = a.TipoArchivo,
            TamanoArchivo = a.TamanoArchivo,
            FechaCarga = a.FechaCarga,
            Estado = a.Estado
        };
    }
}
