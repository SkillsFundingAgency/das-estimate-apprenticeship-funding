using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.Infrastructure.DependencyResolution;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace SFA.DAS.ForecastingTool.Web
{
    public class DependencyConfig
    {
        public static Container RegisterDependencies()
        {
            var container = new WebRegistry().Build();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            return container;
        }
    }
}