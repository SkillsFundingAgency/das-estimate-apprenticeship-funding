using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;

namespace SFA.DAS.ForecastingTool.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["AppInsights:InstrumentationKey"];

            MvcHandler.DisableMvcResponseHeader = true;

            var container = DependencyConfig.RegisterDependencies();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes, container);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-gb");
        }
    }
}
