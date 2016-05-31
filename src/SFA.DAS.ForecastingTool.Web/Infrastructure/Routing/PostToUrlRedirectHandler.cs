using System;
using System.Linq;
using System.Web;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class PostToUrlRedirectHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var form = context.Request.Form;
            var currentUrl = context.Request.Url;
            if (form.AllKeys.Contains("paybillSubmit"))
            {
                Redirect(context, $"{GetUrlToSegment(currentUrl, 1)}{form["paybill"]}");
            }
            else if (form.AllKeys.Contains("trainingCourseSubmit"))
            {
                Redirect(context, $"{GetUrlToSegment(currentUrl, 2)}{form["cohorts"]}x{form["standard"]}");
            }
            else if (form.AllKeys.Contains("trainingCourseSkip"))
            {
                Redirect(context, $"{GetUrlToSegment(currentUrl, 2)}0x0");
            }
        }

        public bool IsReusable { get; } = false;

        private void Redirect(HttpContext context, string relativeUrl)
        {
            var inboundUri = context.Request.Url;
            var baseUri = new Uri($"{inboundUri.Scheme}://{inboundUri.Authority}/");
            var redirectUri = new Uri(baseUri, relativeUrl);
            context.Response.Redirect(redirectUri.ToString());
        }
        private string GetUrlToSegment(Uri url, int segments)
        {
            return url.Segments.Take(segments + 1).Aggregate((x, y) => x + y);
        }
    }
}