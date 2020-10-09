using Azure.Storage.Blobs.Models;
using NodaTime;

namespace WildStrategies.FileManager
{
    public static class AzureBlobExtensions
    {
        public static FileObject ToFileObject(this BlobItem item)
        {
            return new FileObject()
            {
                ContentType = item.Properties.ContentType,
                CreatedTime = Instant.FromDateTimeOffset(item.Properties.CreatedOn.GetValueOrDefault()),
                LastUpdateTime = Instant.FromDateTimeOffset(item.Properties.LastModified.GetValueOrDefault()),
                FullName = item.Name,
                Size = item.Properties.ContentLength.GetValueOrDefault()
            };
        }

        public static FileObject ToFileObject(this BlobProperties item, string fileName)
        {
            return new FileObject()
            {
                ContentType = item.ContentType,
                CreatedTime = Instant.FromDateTimeOffset(item.CreatedOn),
                LastUpdateTime = Instant.FromDateTimeOffset(item.LastModified),
                FullName = fileName,
                Size = item.ContentLength
            };
        }
    }
}
