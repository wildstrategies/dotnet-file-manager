using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public class S3FileManager : S3ReadOnlyFileManager, IFileManager
    {
        public S3FileManager(S3FileManagerSettings settings) : base(settings)
        {
        }

        public async Task DeleteFileAsync(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var response = await client.DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = fileName,
            });

            if (!(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent }).Contains(response.HttpStatusCode))
            {
                throw new Exception($"Unable to delete selected file: {fileName}");
            }
        }

        public Task<Uri> GetUploadFileUriAsync(string fileName, TimeSpan? expiryTime = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
            {
                BucketName = bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.Add(expiryTime ?? TemporaryUrlExpireTime),
                Verb = HttpVerb.PUT,
            };

            return Task.FromResult(new Uri(client.GetPreSignedURL(request)));
        }
    }
}
