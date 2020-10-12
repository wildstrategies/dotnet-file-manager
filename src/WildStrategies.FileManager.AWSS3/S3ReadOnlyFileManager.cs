using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public class S3ReadOnlyFileManager : IReadOnlyFileManager
    {
        private readonly AmazonS3Client client;
        private readonly TimeSpan TemporaryUrlExpireTime;
        private readonly string bucketName;

        public S3ReadOnlyFileManager(S3FilManagerSettings settings)
        {
            client = new AmazonS3Client(
                settings.AccessKey,
                settings.SecretAccessKey,
                Amazon.RegionEndpoint.EnumerableAllRegions.First(x => x.SystemName.Equals(settings.RegionName, StringComparison.InvariantCultureIgnoreCase))
            );

            bucketName = settings.BucketName;
            TemporaryUrlExpireTime = TimeSpan.FromSeconds(settings.TemporaryUrlExpireTime);
        }

        public Task<FileObject> GetFile(string fileName) => ListFilesFromAws(prefix: fileName).ContinueWith(x => x.Result.S3Objects.First().ToFileObject());

        public Task<Uri> GetFileUri(string fileName, TimeSpan? expiryTime = null, bool toDownload = true)
        {
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
            {
                BucketName = bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.Add(expiryTime ?? TemporaryUrlExpireTime),
            };

            request.ResponseHeaderOverrides.ContentDisposition = toDownload ? $"attachment; filename={fileName.Substring(fileName.LastIndexOf("/") + 1)}" : null;
            return Task.FromResult(new Uri(client.GetPreSignedURL(request)));
        }

        private Task<ListObjectsV2Response> ListFilesFromAws(string prefix = null, string continuationToken = null) => client.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request()
        {
            BucketName = bucketName,
            ContinuationToken = continuationToken,
            Prefix = prefix
        });

        public IAsyncEnumerable<FileObject> ListFiles() => ListFiles(null);

        public async IAsyncEnumerable<FileObject> ListFiles(string folder)
        {
            string continuationToken = null;
            while (true)
            {
                ListObjectsV2Response response = await ListFilesFromAws(prefix: folder, continuationToken: continuationToken);
                continuationToken = response.NextContinuationToken;

                foreach (S3Object file in response.S3Objects)
                {
                    if (!file.Key.EndsWith("/"))
                    {
                        yield return new FileObject()
                        {
                            FullName = file.Key
                        };
                    }
                }

                if (continuationToken == null)
                    break;
            }
        }

        public Task<FileObjectMetadataCollection> GetFileMetadata(string fileName) => client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
        {
            BucketName = bucketName,
            Key = fileName
        }).ContinueWith(task =>
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (string key in task.Result.Metadata.Keys)
            {
                output.Add(key, task.Result.Metadata[key]);
            }

            return new FileObjectMetadataCollection(output);
        });
    }
}
