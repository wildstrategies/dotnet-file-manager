using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WildStrategies.FileManager
{
    public sealed class FileObjectMetadataCollection : IEnumerable<FileObjectMetadata>
    {
        private readonly List<FileObjectMetadata> _Collection = new List<FileObjectMetadata>();

        public FileObjectMetadataCollection(IEnumerable<FileObjectMetadata> values) =>
            _Collection.AddRange(values);

        public FileObjectMetadataCollection(IEnumerable<KeyValuePair<string, string>> values) =>
            _Collection.AddRange(values.Select(x => new FileObjectMetadata(x.Key, x.Value)));

        public IEnumerator<FileObjectMetadata> GetEnumerator() => _Collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Collection.GetEnumerator();
    }
}
