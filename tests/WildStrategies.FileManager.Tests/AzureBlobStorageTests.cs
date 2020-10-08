using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WildStrategies.FileManager.Tests
{
    [TestClass]
    public class AzureBlobStorageTests : StorageTestsBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            service = new AzureBlobReadOnlyFileManager(
                Utils.Configuration.GetSection("azure").Get<AzureBlobFileManagerSettings>()
            );
        }
    }
}
