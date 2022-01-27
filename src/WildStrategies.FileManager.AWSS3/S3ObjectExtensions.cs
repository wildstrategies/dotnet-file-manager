using Amazon.S3.Model;

namespace WildStrategies.FileManager
{
    public static class S3ObjectExtensions
    {
        public static FileObject ToFileObject(this S3Object item)
        {
            return new FileObject()
            {
                ContentType = null,
                CreatedTime = item.LastModified,
                LastUpdateTime = item.LastModified,
                FullName = item.Key,
                Size = item.Size
            };
        }
    }
}
