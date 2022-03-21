using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public interface IReadOnlyFileManager
    {
        IAsyncEnumerable<FileObject> ListFilesAsync();
        IAsyncEnumerable<FileObject> ListFilesAsync(string folder);
        Task<FileObject> GetFileAsync(string fileName);
        Task<FileObjectMetadataCollection> GetFileMetadataAsync(string fileName);
        Task<Uri> GetDownloadFileUriAsync(
            string fileName,
            TimeSpan? expiryTime = null,
            bool toDownload = true,
            string attachmentFileName = null
        );
    }
}
