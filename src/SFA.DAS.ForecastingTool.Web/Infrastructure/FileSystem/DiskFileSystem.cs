using System;
using System.IO;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem
{
    public class DiskFileSystem : IFileSystem
    {
        public IFileInfo GetFile(string path)
        {
            return new DiskFileInfo(FixRelativePaths(path));
        }

        private string FixRelativePaths(string path)
        {
            if (!path.StartsWith("~/"))
            {
                return path;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(2));
        }
    }
}