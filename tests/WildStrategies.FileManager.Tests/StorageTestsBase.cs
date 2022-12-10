using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WildStrategies.FileManager.Tests
{
    public abstract class StorageTestsBase
    {
        protected static IFileManager Service { get; set; } = null!;
        private static readonly string basePath = ".unit-tests/file-manager";
        private static readonly string notExistingFile = "8HBfJJtPs86fd1yWJYstszAkLwzw.fne";
        private static readonly HttpClient client = new();
        private static readonly Dictionary<Type, FileObject> fileObjects = new();

        protected static async Task InitEnvironment(TestContext context)
        {
            var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}/files");
            foreach (var file in files)
            {
                var fileContent = await File.ReadAllBytesAsync(file);
                var fileName = $"{basePath}/{Path.GetFileName(file)}";
                var uri = await Service.GetUploadFileUriAsync(fileName);

                var request = new ByteArrayContent(fileContent);
                request.Headers.Add("x-ms-date", DateTime.UtcNow.Ticks.ToString());
                request.Headers.Add("x-ms-blob-type", "BlockBlob");

                var response = await client.PutAsync(uri, request);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    var body = await response.Content.ReadAsStringAsync();
                    context.WriteLine(body);
                }
            }
        }

        protected async Task<FileObject> GetFirstFile()
        {
            if (!fileObjects.ContainsKey(GetType()))
            {
                List<FileObject> files = await Service.ListFilesAsync(basePath).ToList();
                fileObjects.Add(GetType(), files.FirstOrDefault());
            }

            return fileObjects[GetType()];
        }

        [TestMethod]
        public async Task ListFiles()
        {
            List<FileObject> files = await Service.ListFilesAsync(basePath).ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task ListFolderFiles()
        {
            List<FileObject> files = await Service.ListFilesAsync((await GetFirstFile()).FilePath()).ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task GetFile()
        {
            FileObject? file = await Service.GetFileAsync((await GetFirstFile()).FullName);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetNotExistentFile()
        {
            FileObject? file = await Service.GetFileAsync(Guid.NewGuid().ToString());
            Assert.IsNull(file);
        }

        [TestMethod]
        public async Task FileExistsAsync()
        {
            FileObject? file = await Service.GetFileAsync((await GetFirstFile()).FullName);
            Assert.IsNotNull(file);
            if (file != null)
            {
                var exists = await Service.FileExistsAsync(file.Value.FullName);
                Assert.IsTrue(exists);
            }
        }

        [TestMethod]
        public async Task FileNotExistsAsync()
        {
            var exists = await Service.FileExistsAsync(notExistingFile);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public async Task GetFileMetadata()
        {
            FileObjectMetadataCollection? metadata = await Service.GetFileMetadataAsync((await GetFirstFile()).FullName) ?? throw new NullReferenceException();
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        public async Task GetFileUri()
        {
            Uri file = await Service.GetDownloadFileUriAsync((await GetFirstFile()).FullName, toDownload: true) ?? throw new NullReferenceException();
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetFileUriNoDownload()
        {
            Uri file = await Service.GetDownloadFileUriAsync((await GetFirstFile()).FullName, toDownload: false) ?? throw new NullReferenceException();
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task UploadFileWithUri()
        {
            var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}/files");
            foreach (var file in files)
            {
                var fileContent = await File.ReadAllBytesAsync(file);
                var fileName = $"{basePath}/{Path.GetFileName(file)}";

                var request = new ByteArrayContent(fileContent);
                request.Headers.Add("x-ms-date", DateTime.UtcNow.Ticks.ToString());
                request.Headers.Add("x-ms-blob-type", "BlockBlob");

                var uri = await Service.GetUploadFileUriAsync(fileName);
                var response = await client.PutAsync(uri, request);
                response.EnsureSuccessStatusCode();
            }
        }

        [TestMethod]
        public async Task DeleteFile()
        {
            var file = await Service.GetFileAsync((await GetFirstFile()).FullName) ?? throw new NullReferenceException();
            await Service.DeleteFileAsync(file.FileName());
        }
    }
}
