namespace WildStrategies.FileManager
{
    public struct FileObjectMetadata
    {
        public FileObjectMetadata(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
