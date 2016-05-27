namespace SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem
{
    public class DiskFileSystem : IFileSystem
    {
        public IFileInfo GetFile(string path)
        {
            return new DiskFileInfo(path);
        }
    }
}