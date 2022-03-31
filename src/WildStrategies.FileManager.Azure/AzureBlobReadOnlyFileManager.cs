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
        public Task<Uri> GetDownloadFileUriAsync(
            string fileName, 
            TimeSpan? expiryTime = null, 
            bool toDownload = true,
            string attachmentFileName = null
        )
        {
            string contentDisposition = null;

            if (toDownload)
            {
                if (string.IsNullOrWhiteSpace(attachmentFileName))
                {
                    attachmentFileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
                }

                contentDisposition = $"attachment; filename={attachmentFileName}";
            }

            return client.GetFileUriAsync(
                fileName,
                expiryTime,
                contentDisposition,
                false
            );
        }

        public Task<FileObjectMetadataCollection> GetFileMetadataAsync(string fileName) =>
            client.GetBlobMetadata(fileName)
            .ContinueWith(task => new FileObjectMetadataCollection(task.Result), TaskScheduler.Current);

        public Task<bool> FileExistsAsync(string fileName) => client.FileExistsAsync(fileName);
    }
}
