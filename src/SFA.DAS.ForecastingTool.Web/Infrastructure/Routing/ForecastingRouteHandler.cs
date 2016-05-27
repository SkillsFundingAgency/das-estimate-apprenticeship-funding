using System.Web;
using System.Web.Routing;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class ForecastingRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext.HttpContext.Request.Form.Count > 0)
            {
                return new PostToUrlRedirectHandler();
            }
            return new ForecastingMvcHandler(requestContext);
        }
    }
}