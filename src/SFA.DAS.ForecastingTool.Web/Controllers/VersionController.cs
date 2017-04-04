using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Web.Http;
using Microsoft.Azure;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public sealed class VersionController : ApiController
    {
        // GET: api/Version
        public VersionInformation Get()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string assemblyInformationalVersion = fileVersionInfo.ProductVersion;
            return new VersionInformation
            {
                Version = assemblyInformationalVersion,
                AssemblyVersion = version,
                Test = ConfigurationManager.AppSettings["Test"],
                Environment = ConfigurationManager.AppSettings["WorkerRole:EnvironmentName"],
                Environment2 = CloudConfigurationManager.GetSetting("WorkerRole:EnvironmentName")
            };
        }

        public class VersionInformation
        {
            public string BuildId { get; set; }

            public string Version { get; set; }

            public string AssemblyVersion { get; set; }
            public string Environment { get; set; }
            public string Test { get; set; }
            public string Environment2 { get; set; }
        }
    }
}