using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace WildStrategies.FileManager.Tests
{
    [TestClass]
    public class S3StorageTests : StorageTestsBase
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            Service = new S3FileManager(
                Utils.Configuration.GetSection("aws").Get<S3FileManagerSettings>() ?? throw new NullReferenceException()
            );

            await InitEnvironment(context);
        }
    }
}
