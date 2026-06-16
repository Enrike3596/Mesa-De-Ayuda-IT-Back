using Microsoft.AspNetCore.Http;

namespace Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, int ticketId);
        Task DeleteFileAsync(string relativePath);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly string _requestPath;

        public FileStorageService(string basePath, string requestPath)
        {
            _basePath = basePath;
            _requestPath = requestPath.TrimEnd('/');
        }

        public async Task<string> SaveFileAsync(IFormFile file, int ticketId)
        {
            var ticketDir = Path.Combine(_basePath, $"ticket-{ticketId}");
            Directory.CreateDirectory(ticketDir);

            var uniqueName = $"{Guid.NewGuid():N}_{file.FileName}";
            var fullPath = Path.Combine(ticketDir, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{_requestPath}/ticket-{ticketId}/{uniqueName}";
        }

        public Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return Task.CompletedTask;

            var fullPath = Path.Combine(_basePath, relativePath.TrimStart('/'));
            if (relativePath.StartsWith(_requestPath, StringComparison.OrdinalIgnoreCase))
            {
                fullPath = Path.Combine(_basePath, relativePath[_requestPath.Length..].TrimStart('/'));
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}
