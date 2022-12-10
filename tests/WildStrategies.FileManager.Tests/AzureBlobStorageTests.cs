using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WildStrategies.FileManager.Tests
{
    //[TestClass]
    public class AzureBlobStorageTests : StorageTestsBase
    {
        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Initialize(TestContext context)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            service = new AzureBlobFileManager(
                Utils.Configuration.GetSection("azure").Get<AzureBlobFileManagerSettings>() ?? throw new NullReferenceException()
            );
        }
    }
}
