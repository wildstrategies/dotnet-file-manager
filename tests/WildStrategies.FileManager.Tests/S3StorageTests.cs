using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WildStrategies.FileManager.Tests
{
    [TestClass]
    public class S3StorageTests : StorageTestsBase
    {
        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Initialize(TestContext context)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            service = new S3FileManager(
                Utils.Configuration.GetSection("aws").Get<S3FileManagerSettings>() ?? throw new NullReferenceException()
            );
        }
    }
}
