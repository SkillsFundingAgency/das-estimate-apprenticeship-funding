using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Routing;
using SimpleInjector;

namespace SFA.DAS.ForecastingTool.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, Container container)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RouteValueDictionary defaults = new RouteValueDictionary();
            defaults.Add("controller", "Home");
            defaults.Add("action", "Paybill");
            var customRoute = new Route("forecast/{*prms}", defaults, new ForecastingRouteHandler(container));
            routes.Add(customRoute);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Welcome", id = UrlParameter.Optional }
            );
        }
    }
}
