using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WildStrategies.FileManager.Tests
{
    [TestClass]
    internal class S3StorageTests : StorageTestsBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            service = new S3ReadOnlyFileManager();
        }
    }
}
