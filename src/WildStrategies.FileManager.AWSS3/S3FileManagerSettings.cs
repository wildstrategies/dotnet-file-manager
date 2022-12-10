namespace WildStrategies.FileManager
{
    public class S3FileManagerSettings
    {
        public string AccessKey { get; set; } = null!;
        public string SecretAccessKey { get; set; } = null!;
        public string RegionName { get; set; } = null!;
        public string BucketName { get; set; } = null!;
        public int TemporaryUrlExpireTime { get; set; }
    }
}
