using System;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public class AzureBlobFileManager : AzureBlobReadOnlyFileManager, IFileManager
    {
        public AzureBlobFileManager(AzureBlobFileManagerSettings settings) : base(settings)
        {
        }

        public Task DeleteFileAsync(string fileName) => client.DeleteFileAsync(fileName);

        public Task<Uri?> GetUploadFileUriAsync(string fileName, TimeSpan? expiryTime = null) =>
            client.GetFileUriAsync(
                fileName,
                expiryTime,
                null,
                true
            );
    }
}
