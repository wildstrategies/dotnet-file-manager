using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildStrategies.FileManager.Tests
{
    public abstract class StorageTestsBase
    {
        protected static IReadOnlyFileManager service;
        private static readonly Dictionary<Type, FileObject> fileObjects = new Dictionary<Type, FileObject>();

        protected async Task<FileObject> GetFirstFile()
        {
            if (!fileObjects.ContainsKey(GetType()))
            {
                List<FileObject> files = await service.ListFiles().ToList();
                fileObjects.Add(GetType(), files.FirstOrDefault());
            }

            return fileObjects[GetType()];
        }

        [TestMethod]
        public async Task ListFiles()
        {
            List<FileObject> files = await service.ListFiles().ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task ListFolderFiles()
        {
            List<FileObject> files = await service.ListFiles((await GetFirstFile()).FilePath()).ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task GetFile()
        {
            FileObject file = await service.GetFile((await GetFirstFile()).FullName);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetFileMetadata()
        {
            FileObjectMetadataCollection metadata = await service.GetFileMetadata((await GetFirstFile()).FullName);
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        public async Task GetFileUri()
        {
            Uri file = await service.GetFileUri((await GetFirstFile()).FullName, toDownload: true);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetFileUriNoDownload()
        {
            Uri file = await service.GetFileUri((await GetFirstFile()).FullName, toDownload: false);
            Assert.IsNotNull(file);
        }
    }
}
