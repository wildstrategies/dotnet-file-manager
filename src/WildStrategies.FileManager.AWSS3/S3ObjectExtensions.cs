using Amazon.S3.Model;
using NodaTime;

namespace WildStrategies.FileManager
{
    public static class S3ObjectExtensions
    {
        public static FileObject ToFileObject(this S3Object item)
        {
            return new FileObject()
            {
                ContentType = null,
                CreatedTime = Instant.FromDateTimeOffset(item.LastModified),
                LastUpdateTime = Instant.FromDateTimeOffset(item.LastModified),
                FullName = item.Key,
                Size = item.Size
            };
        }
    }
}
