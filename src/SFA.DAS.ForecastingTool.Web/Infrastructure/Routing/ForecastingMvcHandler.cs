using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleInjector;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class ForecastingMvcHandler : MvcHandler
    {
        private readonly Container _container;

        public ForecastingMvcHandler(RequestContext requestContext, Container container) 
            : base(requestContext)
        {
            _container = container;
        }

        protected override IAsyncResult BeginProcessRequest(HttpContextBase httpContext, AsyncCallback callback, object state)
        {
            var parser = _container.GetInstance<UrlParser>();
            var parsedUrl = parser.Parse(httpContext.Request.Url.LocalPath, httpContext.Request.Url.Query);

            RequestContext.RouteData.Values["action"] = parsedUrl.ActionName;
            foreach (var key in parsedUrl.RouteValues.Keys)
            {
                RequestContext.RouteData.Values[key] = parsedUrl.RouteValues[key];
            }

            return base.BeginProcessRequest(httpContext, callback, state);
        }
    }
}