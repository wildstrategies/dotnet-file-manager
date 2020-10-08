using System.Threading.Tasks;

namespace WildStrategies.FileManager
{
    public interface IFileManager : IReadOnlyFileManager
    {
        Task DeleteFile(string fileName);
    }
}
