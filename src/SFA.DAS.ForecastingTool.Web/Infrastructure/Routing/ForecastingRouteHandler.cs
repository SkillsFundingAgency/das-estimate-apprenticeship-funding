using System.Web;
using System.Web.Routing;
using SimpleInjector;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class ForecastingRouteHandler : IRouteHandler
    {
        private readonly Container _container;

        public ForecastingRouteHandler(Container container)
        {
            _container = container;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext.HttpContext.Request.Form.Count > 0)
            {
                return new PostToUrlRedirectHandler();
            }
            return new ForecastingMvcHandler(requestContext, _container);
        }
    }
}