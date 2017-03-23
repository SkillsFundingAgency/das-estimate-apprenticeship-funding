using System;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ForecastingTool.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            var container = DependencyConfig.RegisterDependencies();

            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["AppInsights:InstrumentationKey"];
            var logger = DependencyResolver.Current.GetService<ILog>();

            logger.Info("Starting Web Role");

            SetupApplicationInsights();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes, container);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            logger.Info("Web Role started");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            var logger = DependencyResolver.Current.GetService<SFA.DAS.NLog.Logger.ILog>();

            if (ex is HttpException
                && ((HttpException)ex).GetHttpCode() != 404)
            {
                logger.Error(ex, "App_Error");
            }
        }

        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-gb");
        }

        private void SetupApplicationInsights()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["AppInsights:InstrumentationKey"];

            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ApplicationInsightsInitializer());
        }
    }
}
