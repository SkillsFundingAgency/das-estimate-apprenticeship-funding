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
            var currentUrl = context.Request.Url.LocalPath;
            if (form.AllKeys.Contains("paybillSubmit"))
            {
                Redirect(context, $"forecast/{form["paybill"]}");
            }
            else if (form.AllKeys.Contains("trainingCourseSubmit"))
            {
                Redirect(context, $"{currentUrl}/{form["cohorts"]}x{form["standard"]}");
            }
            else if (form.AllKeys.Contains("trainingCourseSkip"))
            {
                Redirect(context, $"{currentUrl}/0x0");
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
    }
}