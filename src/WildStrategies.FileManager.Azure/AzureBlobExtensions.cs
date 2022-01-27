using Azure.Storage.Blobs.Models;

namespace WildStrategies.FileManager
{
    public static class AzureBlobExtensions
    {
        public static FileObject ToFileObject(this BlobItem item)
        {
            return new FileObject()
            {
                ContentType = item.Properties.ContentType,
                CreatedTime = item.Properties.CreatedOn.GetValueOrDefault(),
                LastUpdateTime = item.Properties.LastModified.GetValueOrDefault(),
                FullName = item.Name,
                Size = item.Properties.ContentLength.GetValueOrDefault()
            };
        }

        public static FileObject ToFileObject(this BlobProperties item, string fileName)
        {
            return new FileObject()
            {
                ContentType = item.ContentType,
                CreatedTime = item.CreatedOn,
                LastUpdateTime = item.LastModified,
                FullName = fileName,
                Size = item.ContentLength
            };
        }
    }
}
