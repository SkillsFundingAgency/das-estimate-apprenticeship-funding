using System.Web;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class CatchAllHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("/error/not-found.htm");
        }

        public bool IsReusable { get; }
    }
}