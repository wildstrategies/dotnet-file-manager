using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace WildStrategies.FileManager.Tests
{
    public abstract class StorageTestsBase
    {
        protected static IReadOnlyFileManager service;
        private FileObject _FirstFile;

        protected async Task<FileObject> GetFirstFile()
        {
            if (_FirstFile == null)
            {
                var files = await service.ListFiles().ToList();
                _FirstFile = files.FirstOrDefault();
            }

            return _FirstFile;
        }

        [TestMethod]
        public async Task ListFiles()
        {
            var files = await service.ListFiles().ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task ListFolderFiles()
        {
            var files = await service.ListFiles((await GetFirstFile()).FilePath()).ToList();
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task GetFile()
        {
            var file = await service.GetFile((await GetFirstFile()).FullName);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetFileMetadata()
        {
            var metadata = await service.GetFileMetadata((await GetFirstFile()).FullName);
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        public async Task GetFileUri()
        {
            var file = await service.GetFileUri((await GetFirstFile()).FullName, toDownload: true);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task GetFileUriNoDownload()
        {
            var file = await service.GetFileUri((await GetFirstFile()).FullName, toDownload: false);
            Assert.IsNotNull(file);
        }
    }
}
