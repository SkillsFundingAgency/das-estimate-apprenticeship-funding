using System.Web;
using System.Web.Routing;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class CatchAllRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new CatchAllHttpHandler();
        }
    }
}