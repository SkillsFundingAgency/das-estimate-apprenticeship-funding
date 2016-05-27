using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class ForecastingMvcHandler : MvcHandler
    {
        public ForecastingMvcHandler(RequestContext requestContext) 
            : base(requestContext)
        {
        }

        protected override IAsyncResult BeginProcessRequest(HttpContextBase httpContext, AsyncCallback callback, object state)
        {
            var parsedUrl = UrlParser.Parse(httpContext.Request.Url.LocalPath);

            RequestContext.RouteData.Values["action"] = parsedUrl.ActionName;
            foreach (var key in parsedUrl.RouteValues.Keys)
            {
                RequestContext.RouteData.Values[key] = parsedUrl.RouteValues[key];
            }

            return base.BeginProcessRequest(httpContext, callback, state);
        }
    }
}