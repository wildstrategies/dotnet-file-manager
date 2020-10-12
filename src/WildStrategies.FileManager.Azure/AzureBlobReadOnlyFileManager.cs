﻿using System;
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

        public IAsyncEnumerable<FileObject> ListFiles() => ListFilesFromAzure(null);
        public IAsyncEnumerable<FileObject> ListFiles(string folder) => ListFilesFromAzure(folder);
        public Task<FileObject> GetFile(string fileName) => client.GetFile(fileName);
        public Task<Uri> GetFileUri(string fileName, TimeSpan? expiryTime = null, bool toDownload = true) =>
            client.GetFileUriAsync(
                fileName,
                expiryTime,
                toDownload ? $"attachment; filename={fileName.Substring(fileName.LastIndexOf("/") + 1)}" : null
            );

        public Task<FileObjectMetadataCollection> GetFileMetadata(string fileName) =>
            client.GetBlobMetadata(fileName)
            .ContinueWith(task => new FileObjectMetadataCollection(task.Result), TaskScheduler.Current);
    }
}
