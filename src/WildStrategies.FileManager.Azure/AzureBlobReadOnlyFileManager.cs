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
            await foreach (var file in client.SearchFiles(prefix))
            {
                yield return file.ToFileObject();
            }
        }

        public IAsyncEnumerable<FileObject> ListFiles() => ListFilesFromAzure(null);
        public IAsyncEnumerable<FileObject> ListFiles(string folder) => ListFilesFromAzure(folder);
        public async Task<FileObject> GetFile(string fileName)
        {
            var enumerator = ListFilesFromAzure(fileName).GetAsyncEnumerator();
            if (await enumerator.MoveNextAsync())
            {
                return enumerator.Current;
            }

            throw new Exception();
        }

        public Task<Uri> GetFileUri(string fileName, TimeSpan? expiryTime = null, bool toDownload = true) =>
            client.GetFileUriAsync(
                fileName,
                expiryTime,
                toDownload ? $"attachment; filename={fileName.Substring(fileName.LastIndexOf("/") + 1)}" : null
            );
    }
}
