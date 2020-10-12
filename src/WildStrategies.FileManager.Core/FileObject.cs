using NodaTime;

namespace WildStrategies.FileManager
{
    public struct FileObject
    {
        public string FullName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public Instant CreatedTime { get; set; }
        public Instant LastUpdateTime { get; set; }
    }
}
