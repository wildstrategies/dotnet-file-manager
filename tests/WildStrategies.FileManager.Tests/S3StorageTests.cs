using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
