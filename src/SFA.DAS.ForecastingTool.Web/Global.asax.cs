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
        private ILog _logger;

        public MvcApplication()
        {
            _logger = DependencyResolver.Current.GetService<ILog>();
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            var container = DependencyConfig.RegisterDependencies();

            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["AppInsights:InstrumentationKey"];

            _logger = DependencyResolver.Current.GetService<ILog>();

            _logger.Info("Starting Web Role");

            SetupApplicationInsights();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes, container);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _logger.Info("Web Role started");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            _logger = DependencyResolver.Current.GetService<ILog>();

            if (ex is HttpException
                && ((HttpException)ex).GetHttpCode() != 404)
            {
                _logger.Error(ex, "App_Error");
            }
        }

        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-gb");

            _logger = DependencyResolver.Current.GetService<ILog>();

            HttpContext context = base.Context;

            if (!context.Request.Path.StartsWith("/__browserlink"))
            {
                _logger.Info($"{context.Request.HttpMethod} {context.Request.Path}");
            }
        }

        private void SetupApplicationInsights()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["AppInsights:InstrumentationKey"];

            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ApplicationInsightsInitializer());
        }
    }
}
