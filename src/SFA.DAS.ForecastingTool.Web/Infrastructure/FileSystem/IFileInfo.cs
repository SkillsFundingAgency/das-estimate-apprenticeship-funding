using System.IO;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem
{
    public interface IFileInfo
    {
        string Path { get; }
        bool Exists { get; }
        long Length { get; }

        Stream OpenRead();
    }
}