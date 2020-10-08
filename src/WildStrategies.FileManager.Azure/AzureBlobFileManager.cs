using System;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public class AzureBlobFileManager : AzureBlobReadOnlyFileManager, IFileManager
    {
        public AzureBlobFileManager(AzureBlobFileManagerSettings settings) : base(settings)
        {
        }

        public Task DeleteFile(string fileName) => client.DeleteFileAsync(fileName);
    }
}
