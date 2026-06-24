using DTOs;
using Helpers;
using Microsoft.AspNetCore.Http;
using Models;
using Repositories;

namespace Services
{
    public interface ITicketAnexoService
    {
        Task<List<TicketAnexoResponseDTO>> ObtenerTodosAsync();
        Task<List<TicketAnexoResponseDTO>> ObtenerPorTicketIdAsync(int ticketId);
        Task<TicketAnexoResponseDTO?> ObtenerPorIdAsync(int id);
        Task<TicketAnexoResponseDTO> CrearAsync(TicketAnexoCreateDTO dto);
        Task<TicketAnexoResponseDTO> SubirArchivoAsync(IFormFile archivo, int ticketId, int usuarioId);
        Task<TicketAnexoResponseDTO?> ActualizarAsync(int id, TicketAnexoUpdateDTO dto);
        Task<bool> EliminarAsync(int id);
    }

    public class TicketAnexoService : ITicketAnexoService
    {
        private readonly ITicketAnexosRepository _repo;
        private readonly IFileStorageService _fileStorage;
        private readonly ITicketRepository _ticketRepo;

        public TicketAnexoService(ITicketAnexosRepository repo, IFileStorageService fileStorage, ITicketRepository ticketRepo)
        {
            _repo = repo;
            _fileStorage = fileStorage;
            _ticketRepo = ticketRepo;
        }

        public async Task<List<TicketAnexoResponseDTO>> ObtenerTodosAsync()
        {
            var anexos = await _repo.ObtenerTodosAsync();
            return anexos.Select(MapearRespuesta).ToList();
        }

        public async Task<List<TicketAnexoResponseDTO>> ObtenerPorTicketIdAsync(int ticketId)
        {
            var anexos = await _repo.ObtenerPorTicketIdAsync(ticketId);
            return anexos.Select(MapearRespuesta).ToList();
        }

        public async Task<TicketAnexoResponseDTO?> ObtenerPorIdAsync(int id)
        {
            var anexo = await _repo.ObtenerPorIdAsync(id);
            return anexo == null ? null : MapearRespuesta(anexo);
        }

        public async Task<TicketAnexoResponseDTO> CrearAsync(TicketAnexoCreateDTO dto)
        {
            var anexo = new TicketAnexo
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

        public async Task<TicketAnexoResponseDTO> SubirArchivoAsync(IFormFile archivo, int ticketId, int usuarioId)
        {
            var ticket = await _ticketRepo.ObtenerPorIdAsync(ticketId);
            if (ticket == null)
                throw new ArgumentException($"El ticket con ID {ticketId} no existe.", nameof(ticketId));

            if (TicketEstadoNormalizer.Normalize(ticket.Estado) == TicketEstadoNormalizer.Cerrado)
                throw new InvalidOperationException("No se pueden adjuntar archivos a un ticket cerrado.");

            var url = await _fileStorage.SaveFileAsync(archivo, ticketId);

            var anexo = new TicketAnexo
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

        public async Task<TicketAnexoResponseDTO?> ActualizarAsync(int id, TicketAnexoUpdateDTO dto)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto);
            return actualizado == null ? null : MapearRespuesta(actualizado);
        }

        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        private static TicketAnexoResponseDTO MapearRespuesta(TicketAnexo a) => new()
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
