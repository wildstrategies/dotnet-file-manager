using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace WildStrategies.FileManager.Tests
{
    [TestClass]
    public class AzureBlobStorageTests : StorageTestsBase
    {
        [ClassInitialize]
        public async static Task Initialize(TestContext context)
        {
            Service = new AzureBlobFileManager(
                Utils.Configuration.GetSection("azure").Get<AzureBlobFileManagerSettings>() ?? throw new NullReferenceException()
            );

            await InitEnvironment(context);
        }
    }
}
