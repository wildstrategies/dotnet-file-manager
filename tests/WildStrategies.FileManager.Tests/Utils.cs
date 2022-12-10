using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace WildStrategies.FileManager.Tests
{
    public static class UtilsExtensions
    {
        public static async Task<List<FileObject>> ToList(this IAsyncEnumerable<FileObject> files)
        {
            List<FileObject> output = new();
            await foreach (FileObject file in files)
            {
                output.Add(file);
            }

            return output;
        }
    }

    [TestClass]
    public class Utils
    {
        public static TestContext TestContext { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            TestContext = context;
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json", true)
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
        }

        [AssemblyCleanup]
        public static void CleanUp()
        {
        }
    }
}
