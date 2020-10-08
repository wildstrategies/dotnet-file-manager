using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public class S3ReadOnlyFileManager : IReadOnlyFileManager
    {
        private readonly AmazonS3Client client = new AmazonS3Client(
            "accessKey",
            "accessSecret",
            new AmazonS3Config()
            {
                ServiceURL = "http://localhost:9444",
                ForcePathStyle = true
            });

        public S3ReadOnlyFileManager()
        {
        }

        public Task<FileObject> GetFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GetFileUri(string fileName, bool toDownload)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<FileObject> ListFiles()
        {
            var output = await client.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request()
            {
                BucketName = "test-bucket",
            });

            foreach (var file in output.S3Objects)
            {
                yield return new FileObject()
                {
                    FullName = file.Key
                };
            }
        }

        public IAsyncEnumerable<FileObject> ListFiles(string folder)
        {
            throw new NotImplementedException();
        }
    }
}
