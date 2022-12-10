using System;

namespace WildStrategies.FileManager
{
    public readonly struct FileObject
    {
        public string FullName { get; init; }
        public string? ContentType { get; init; }
        public long Size { get; init; }
        public DateTimeOffset CreatedTime { get; init; }
        public DateTimeOffset LastUpdateTime { get; init; }
    }
}
