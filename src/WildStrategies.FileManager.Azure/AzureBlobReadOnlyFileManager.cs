using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public class AzureBlobReadOnlyFileManager : IReadOnlyFileManager
    {
        internal readonly AzureBlobStorageClient client;

        public AzureBlobReadOnlyFileManager(AzureBlobFileManagerSettings settings)
        {
            client = new AzureBlobStorageClient(settings);
        }

        private async IAsyncEnumerable<FileObject> ListFilesFromAzure(string prefix)
        {
            await foreach (Azure.Storage.Blobs.Models.BlobItem file in client.SearchFiles(prefix))
            {
                yield return file.ToFileObject();
            }
        }

        public IAsyncEnumerable<FileObject> ListFilesAsync() => ListFilesFromAzure(null);
        public IAsyncEnumerable<FileObject> ListFilesAsync(string folder) => ListFilesFromAzure(folder);
        public Task<FileObject> GetFileAsync(string fileName) => client.GetFile(fileName);
        public Task<Uri> GetDownloadFileUriAsync(string fileName, TimeSpan? expiryTime = null, bool toDownload = true) =>
            client.GetFileUriAsync(
                fileName,
                expiryTime,
                toDownload ? $"attachment; filename={fileName.Substring(fileName.LastIndexOf("/") + 1)}" : null,
                false
            );

        public Task<FileObjectMetadataCollection> GetFileMetadataAsync(string fileName) =>
            client.GetBlobMetadata(fileName)
            .ContinueWith(task => new FileObjectMetadataCollection(task.Result), TaskScheduler.Current);
    }
}
