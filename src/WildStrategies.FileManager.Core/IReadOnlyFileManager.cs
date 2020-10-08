using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public interface IReadOnlyFileManager
    {
        IAsyncEnumerable<FileObject> ListFiles();
        IAsyncEnumerable<FileObject> ListFiles(string folder);
        Task<FileObject> GetFile(string fileName);
        Task<Uri> GetFileUri(string fileName, TimeSpan? expiryTime = null, bool toDownload = true);
    }
}
