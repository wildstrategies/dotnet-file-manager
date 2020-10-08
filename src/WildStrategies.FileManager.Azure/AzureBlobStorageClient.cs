using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public static class AzureBlobExtensions
    {
        public static FileObject ToFileObject(this BlobItem item)
        {
            return new FileObject()
            {
                Abstract = item.Metadata != null && item.Metadata.ContainsKey("Abstract") ? item.Metadata["Abstract"] : null,
                ContentType = item.Properties.ContentType,
                CreatedTime = Instant.FromDateTimeOffset(item.Properties.CreatedOn.GetValueOrDefault()),
                LastUpdateTime = Instant.FromDateTimeOffset(item.Properties.LastModified.GetValueOrDefault()),
                FullName = item.Name,
                Size = item.Properties.ContentLength.GetValueOrDefault()
            };
        }
    }

    internal class AzureBlobStorageClient
    {
        //private readonly BlobServiceClient serviceClient;
        private readonly StorageSharedKeyCredential Credentials;
        private readonly BlobContainerClient containerClient;

        private readonly TimeSpan TemporaryUrlExpireTime;

        public AzureBlobStorageClient(AzureBlobFileManagerSettings settings)
        {
            Credentials = new StorageSharedKeyCredential(settings.AccountName, settings.AccountKey);
            containerClient = new BlobServiceClient(settings.StorageUri, Credentials).GetBlobContainerClient(settings.ContainerName);
            TemporaryUrlExpireTime = TimeSpan.FromSeconds(settings.TemporaryUrlExpireTime);
        }

        public IAsyncEnumerable<BlobItem> SearchFiles(string prefix) => containerClient.GetBlobsAsync(prefix: prefix, traits: BlobTraits.All);

        public Task<Uri> GetFileUriAsync(string fileName, string contentDisposition = null)
        {
            BlobSasBuilder builder = new BlobSasBuilder()
            {
                StartsOn = DateTimeOffset.UtcNow.AddSeconds(-2),
                ExpiresOn = DateTimeOffset.UtcNow.Add(TemporaryUrlExpireTime),
                ContentDisposition = contentDisposition,
                BlobContainerName = containerClient.Name,
                BlobName = fileName
            };

            builder.SetPermissions(BlobSasPermissions.Read);

            return Task.FromResult(
                new Uri($"{containerClient.GetBlobClient(fileName).Uri}?{builder.ToSasQueryParameters(Credentials)}")
            );
        }


        //public Task<BlobItem> GetFileAsync(string bucketName, string fileName) => GetFilesAsync(bucketName, fileName)
        //    .ContinueWith(x => x.Result.First(), TaskScheduler.Current);

        //public async Task<byte[]> GetFileContentAsync(string bucketName, string fileName)
        //{
        //    using MemoryStream ms = new MemoryStream();
        //    await serviceClient.GetBlobContainerClient(bucketName).GetBlobClient(fileName).DownloadToAsync(ms).ConfigureAwait(false);

        //    return ms.ToArray();
        //}


        //public Task UploadFileAsync(
        //    string bucketName,
        //    string fileName,
        //    string contentType,
        //    Stream contentStream) =>
        //    // TODO: Set contentType
        //    serviceClient.GetBlobContainerClient(bucketName).UploadBlobAsync(fileName, contentStream).ContinueWith(res =>
        //    {
        //        return Task.FromResult(true);
        //    }, TaskScheduler.Default);

        //public Task SetFileMetadata(string bucketName, string fileName, IDictionary<string, string> metadata) =>
        //    serviceClient.GetBlobContainerClient(bucketName).GetBlobClient(fileName).SetMetadataAsync(metadata ?? throw new ArgumentNullException(nameof(metadata)));

        //private Task<Uri> GetTemporaryUriAsync(string bucketName, string fileName, string contentDisposition = null)
        //{
        //    BlobSasBuilder builder = new BlobSasBuilder()
        //    {
        //        StartsOn = DateTimeOffset.UtcNow.AddSeconds(-2),
        //        ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(UrlExpiryTimeInSeconds - 1),
        //        ContentDisposition = contentDisposition,
        //        BlobContainerName = bucketName,
        //        BlobName = fileName
        //    };

        //    builder.SetPermissions(BlobSasPermissions.Read);

        //    return Task.FromResult(new Uri($"{serviceClient.GetBlobContainerClient(bucketName).GetBlockBlobClient(fileName).Uri}?{builder.ToSasQueryParameters(this.credentials)}"));
        //}

        //public Task<Uri> GetPlacholderUriAsync(string bucketName, string fileName) =>
        //    GetTemporaryUriAsync(bucketName, fileName);

        //public Task<Uri> GetFileDownloadUriAsync(string bucketName, string fileName) =>
        //    GetTemporaryUriAsync(bucketName, fileName, $"attachment; filename={fileName.Substring(fileName.LastIndexOf("/") + 1)}");


        //public Task DeleteFileAsync(string bucketName, string fileName)
        //    => serviceClient.GetBlobContainerClient(bucketName).GetBlobClient(fileName).DeleteIfExistsAsync();

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: dispose managed state (managed objects)
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        //        // TODO: set large fields to null
        //        disposedValue = true;
        //    }
        //}

        //// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //// ~AzureStorageClient()
        //// {
        ////     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        ////     Dispose(disposing: false);
        //// }

        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
