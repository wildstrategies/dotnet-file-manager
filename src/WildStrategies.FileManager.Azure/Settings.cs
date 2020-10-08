using System;
using System.Collections.Generic;
using System.Text;

namespace WildStrategies.FileManager
{
    public class AzureFileManagerSettings
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
    }

    public class AzureBlobFileManagerSettings : AzureFileManagerSettings
    {
        public string ContainerName { get; set; }
        public Uri StorageUri { get; set; }
        public int TemporaryUrlExpireTime { get; set; }
    }
}
