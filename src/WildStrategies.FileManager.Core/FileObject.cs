using NodaTime;

namespace WildStrategies.FileManager
{
    public struct FileObject
    {
        public string FullName { get; init; }
        public string ContentType { get; init; }
        public long Size { get; init; }
        public Instant CreatedTime { get; init; }
        public Instant LastUpdateTime { get; init; }
    }
}
