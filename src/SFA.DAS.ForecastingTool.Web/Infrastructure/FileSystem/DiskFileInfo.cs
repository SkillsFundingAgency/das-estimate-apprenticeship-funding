using System.IO;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem
{
    public class DiskFileInfo : IFileInfo
    {
        public DiskFileInfo(string path)
        {
            Path = path;

            var info = new FileInfo(path);
            Exists = info.Exists;
            if (Exists)
            {
                Length = info.Length;
            }
        }
        public string Path { get; }
        public bool Exists { get; }
        public long Length { get; }

        public Stream OpenRead(FileShare share = FileShare.None)
        {
            return new FileStream(Path, FileMode.Open, FileAccess.Read, share);
        }
    }
}