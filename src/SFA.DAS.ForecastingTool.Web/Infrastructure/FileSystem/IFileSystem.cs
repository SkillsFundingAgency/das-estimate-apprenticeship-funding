namespace SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        IFileInfo GetFile(string path);
    }
}