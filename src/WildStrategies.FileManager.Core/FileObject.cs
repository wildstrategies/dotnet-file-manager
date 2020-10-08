using NodaTime;
using System;
using System.IO;

namespace WildStrategies.FileManager
{
    public static class FileObjectExtension
    {
        public static string ToHumanFriendlyString(this FileObject fileObject)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (fileObject.Size == 0)
            {
                return "0 " + suf[0];
            }

            long bytes = Math.Abs((long)fileObject.Size);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign((long)fileObject.Size) * num).ToString(System.Globalization.CultureInfo.CurrentCulture) + " " + suf[place];
        }

        public static string FileExtension(this FileObject fileObject) => Path.GetExtension(fileObject.FullName)?.Replace(".", "")?.ToLower(System.Globalization.CultureInfo.InvariantCulture);
        public static string FileName(this FileObject fileObject) => Path.GetFileName(fileObject.FullName);
        public static string FilePath(this FileObject fileObject) => Path.GetDirectoryName(fileObject.FullName);
    }

    public class FileObject
    {
        public string FullName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string Abstract { get; set; }
        public Instant CreatedTime { get; set; }
        public Instant LastUpdateTime { get; set; }
    }
}
