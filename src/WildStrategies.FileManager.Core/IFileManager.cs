using System;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public interface IFileManager : IReadOnlyFileManager
    {
        Task DeleteFileAsync(string fileName);
        Task<Uri> GetUploadFileUriAsync(string fileName, TimeSpan? expiryTime = null);
    }
}
