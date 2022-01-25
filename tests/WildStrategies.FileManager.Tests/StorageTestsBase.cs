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
        protected static IFileManager service;
        private static readonly string basePath = ".unit-tests";
        private static readonly HttpClient client = new HttpClient();
        private static readonly Dictionary<Type, FileObject> fileObjects = new Dictionary<Type, FileObject>();

        protected async Task<FileObject> GetFirstFile()
        {
            if (!fileObjects.ContainsKey(GetType()))
            {
                List<FileObject> files = await service.ListFilesAsync(basePath).ToList();
                fileObjects.Add(GetType(), files.FirstOrDefault());
            }

            return fileObjects[GetType()];
        }

        [TestMethod]
        public async Task ListFiles()
        {
            List<FileObject> files = await service.ListFilesAsync(basePath).ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task ListFolderFiles()
        {
            List<FileObject> files = await service.ListFilesAsync((await GetFirstFile()).FilePath()).ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task GetFile()
        {
            FileObject file = await service.GetFileAsync((await GetFirstFile()).FullName);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetFileMetadata()
        {
            FileObjectMetadataCollection metadata = await service.GetFileMetadataAsync((await GetFirstFile()).FullName);
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        public async Task GetFileUri()
        {
            Uri file = await service.GetDownloadFileUriAsync((await GetFirstFile()).FullName, toDownload: true);
            Assert.IsNotNull(file);
            //var result = await client.GetAsync(file);
            //result.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task GetFileUriNoDownload()
        {
            Uri file = await service.GetDownloadFileUriAsync((await GetFirstFile()).FullName, toDownload: false);
            Assert.IsNotNull(file);
            //var result = await client.GetAsync(file);
            //result.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task UploadFileWithUri()
        {
            var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}/files");
            foreach (var file in files)
            {
                var fileContent = await File.ReadAllBytesAsync(file);
                var fileName = $"{basePath}/{Path.GetFileName(file)}";
                var uri = await service.GetUploadFileUriAsync(fileName);

                var request = new ByteArrayContent(fileContent);

                await client.PutAsync(uri, request);
            }
        }

        [TestMethod]
        public async Task DeleteFile()
        {
            var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}/files");
            foreach (var file in files)
            {
                var fileContent = await File.ReadAllBytesAsync(file);
                var fileName = $"{basePath}/{Path.GetFileName(file)}";
                var uri = await service.GetUploadFileUriAsync(fileName);

                var request = new ByteArrayContent(fileContent);

                await client.PutAsync(uri, request);
                await service.DeleteFileAsync(fileName);
            }
        }
    }
}
