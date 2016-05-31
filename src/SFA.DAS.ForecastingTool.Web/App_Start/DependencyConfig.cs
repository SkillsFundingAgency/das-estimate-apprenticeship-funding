using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.Infrastructure.DependencyResolution;
using SimpleInjector.Integration.Web.Mvc;

namespace SFA.DAS.ForecastingTool.Web
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var container = new ContainerBuilder().Build();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}