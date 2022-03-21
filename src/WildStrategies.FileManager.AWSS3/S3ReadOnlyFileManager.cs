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
        protected readonly AmazonS3Client client;
        protected readonly TimeSpan TemporaryUrlExpireTime;
        protected readonly string bucketName;

        public S3ReadOnlyFileManager(S3FileManagerSettings settings)
        {
            client = new AmazonS3Client(
                settings.AccessKey,
                settings.SecretAccessKey,
                Amazon.RegionEndpoint.EnumerableAllRegions.First(x => x.SystemName.Equals(settings.RegionName, StringComparison.InvariantCultureIgnoreCase))
            );

            bucketName = settings.BucketName;
            TemporaryUrlExpireTime = TimeSpan.FromSeconds(settings.TemporaryUrlExpireTime);
        }

        public Task<FileObject> GetFileAsync(string fileName) => ListFilesFromAws(prefix: fileName).ContinueWith(x => x.Result.S3Objects.First().ToFileObject());

        public Task<Uri> GetDownloadFileUriAsync(
            string fileName, 
            TimeSpan? expiryTime = null, 
            bool toDownload = true,
            string attachmentFileName = null
        )
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
                Verb = HttpVerb.GET
            };

            if (toDownload)
            {
                if (string.IsNullOrWhiteSpace(attachmentFileName))
                {
                    attachmentFileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
                }

                request.ResponseHeaderOverrides.ContentDisposition = toDownload ? $"attachment; filename={attachmentFileName}" : null;
            }

            return Task.FromResult(new Uri(client.GetPreSignedURL(request)));
        }

        private Task<ListObjectsV2Response> ListFilesFromAws(string prefix = null, string continuationToken = null) => client.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request()
        {
            BucketName = bucketName,
            ContinuationToken = continuationToken,
            Prefix = prefix
        });

        public IAsyncEnumerable<FileObject> ListFilesAsync() => ListFilesAsync(null);

        public async IAsyncEnumerable<FileObject> ListFilesAsync(string folder)
        {
            string continuationToken = null;
            while (true)
            {
                ListObjectsV2Response response = await ListFilesFromAws(prefix: folder, continuationToken: continuationToken);
                continuationToken = response.NextContinuationToken;

                foreach (S3Object file in response.S3Objects)
                {
                    if (!file.Key.EndsWith("/") && !file.Key.EndsWith("_$folder$"))
                    {
                        yield return file.ToFileObject();
                    }
                }

                if (continuationToken == null)
                    break;
            }
        }

        public Task<FileObjectMetadataCollection> GetFileMetadataAsync(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            return client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
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
}
