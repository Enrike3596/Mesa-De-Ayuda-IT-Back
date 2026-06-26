using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _container;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureBlobStorage:ConnectionString no está configurado.");

            var containerName = configuration["AzureBlobStorage:ContainerName"]
                ?? "ticket-attachments";

            var blobService = new BlobServiceClient(connectionString);
            _container = blobService.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> SaveFileAsync(IFormFile file, int ticketId)
        {
            var blobName = $"ticket-{ticketId}/{Guid.NewGuid():N}_{file.FileName}";
            var blob = _container.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blob.Uri.ToString();
        }

        public async Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var uri = new Uri(relativePath);
            var blobName = uri.LocalPath.TrimStart('/').TrimStart('/');
            var segments = blobName.Split('/');
            if (segments.Length < 2)
                return;

            var blob = _container.GetBlobClient(string.Join("/", segments.Skip(1)));
            await blob.DeleteIfExistsAsync();
        }
    }
}
