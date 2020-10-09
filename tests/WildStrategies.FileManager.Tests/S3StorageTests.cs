using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WildStrategies.FileManager.Tests
{
    [TestClass]
    public class S3StorageTests : StorageTestsBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            service = new S3ReadOnlyFileManager(
                Utils.Configuration.GetSection("aws").Get<S3FilManagerSettings>()
            );
        }
    }
}
