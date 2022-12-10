using System;

namespace WildStrategies.FileManager
{
    public class AzureFileManagerSettings
    {
        public string AccountName { get; set; } = null!;
        public string AccountKey { get; set; } = null!;
    }

    public class AzureBlobFileManagerSettings : AzureFileManagerSettings
    {
        private Uri _storageUri = null!;

        public string ContainerName { get; set; } = null!;
        public Uri StorageUri
        {
            get => _storageUri != null ? new Uri($"{_storageUri}{AccountName}") : new Uri($"https://{AccountName ?? "TEMP"}.blob.core.windows.net/");
            set => _storageUri = value;
        }
        public int TemporaryUrlExpireTime { get; set; }
    }
}
