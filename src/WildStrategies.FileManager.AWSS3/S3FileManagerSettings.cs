namespace WildStrategies.FileManager
{
    public class S3FileManagerSettings
    {
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
        public string RegionName { get; set; }
        public string BucketName { get; set; }
        public int TemporaryUrlExpireTime { get; set; }
    }
}
