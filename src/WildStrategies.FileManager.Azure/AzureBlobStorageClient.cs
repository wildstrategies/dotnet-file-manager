using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{

    internal class AzureBlobStorageClient
    {
        //private readonly BlobServiceClient serviceClient;
        private readonly StorageSharedKeyCredential Credentials;
        private readonly BlobContainerClient containerClient;
        private readonly BlobServiceClient serviceClient;

        private readonly TimeSpan TemporaryUrlExpireTime;

        public AzureBlobStorageClient(AzureBlobFileManagerSettings settings)
        {
            Credentials = new StorageSharedKeyCredential(settings.AccountName, settings.AccountKey);
            serviceClient = new BlobServiceClient(settings.StorageUri, Credentials);
            containerClient = serviceClient.GetBlobContainerClient(settings.ContainerName);
            TemporaryUrlExpireTime = TimeSpan.FromSeconds(settings.TemporaryUrlExpireTime);
        }

        public IAsyncEnumerable<BlobItem> SearchFiles(string? prefix) => 
            containerClient.GetBlobsAsync(prefix: prefix, traits: BlobTraits.None);

        public async Task<FileObject?> GetFile(string fileName)
        {
            if (!await containerClient.ExistsAsync())
            {
                return null;
            }
            return await containerClient.GetBlobClient(fileName).GetPropertiesAsync()
            .ContinueWith(task => task.Result.Value.ToFileObject(fileName));
        }

        public Task<Uri> GetFileUriAsync(string fileName, TimeSpan? expiryTime, string? contentDisposition, bool write)
        {
            BlobSasBuilder builder = new()
            {
                StartsOn = DateTimeOffset.UtcNow.AddSeconds(-2),
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime ?? TemporaryUrlExpireTime),
                ContentDisposition = contentDisposition,
                BlobContainerName = containerClient.Name,
                BlobName = fileName
            };

            if (write)
            {
                builder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
            }
            else
            {
                builder.SetPermissions(BlobSasPermissions.Read);
            }

            return Task.FromResult(
                new Uri($"{containerClient.GetBlobClient(fileName).Uri}?{builder.ToSasQueryParameters(Credentials)}")
            );
        }

        public Task<bool> FileExistsAsync(string fileName) =>
            containerClient.GetBlobClient(fileName).ExistsAsync().ContinueWith(task => task.Result.Value);

        public async Task<IEnumerable<KeyValuePair<string, string>>?> GetBlobMetadata(string fileName)
        {
            if (!await containerClient.GetBlobClient(fileName).ExistsAsync())
                return null;
            return await containerClient.GetBlobClient(fileName).GetPropertiesAsync().ContinueWith(task => task.Result.Value.Metadata.AsEnumerable());
        }

        public Task<bool> DeleteFileAsync(string fileName)
            => containerClient.GetBlobClient(fileName).DeleteIfExistsAsync().ContinueWith(res => res.Result.Value);

    }
}
