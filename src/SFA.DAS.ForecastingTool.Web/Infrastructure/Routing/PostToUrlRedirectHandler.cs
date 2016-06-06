using System;
using System.Collections.Specialized;
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
                var paybillEntry = GetFormValue(form, "paybill", "0");
                if (PaybillIsEligibleForLevy(paybillEntry))
                {
                    Redirect(context, $"{GetUrlToSegment(currentUrl, 1)}{paybillEntry}");
                }
                else
                {
                    Redirect(context, $"{GetUrlToSegment(currentUrl, 1)}{paybillEntry}/1");
                }
            }
            else if (form.AllKeys.Contains("englishFractionSubmit"))
            {
                var englishFractionEntry = GetFormValue(form, "englishFraction", "0");
                Redirect(context, $"{GetUrlToSegment(currentUrl, 2)}{englishFractionEntry}");
            }
            else if (form.AllKeys.Contains("englishFractionSkip"))
            {
                Redirect(context, $"{GetUrlToSegment(currentUrl, 2)}100");
            }
            else if (form.AllKeys.Contains("trainingCourseSubmit"))
            {
                var cohortsEntry = GetFormValue(form, "cohorts", "0");
                var standardSelection = GetFormValue(form, "standard", "0");
                Redirect(context, $"{GetUrlToSegment(currentUrl, 3)}{cohortsEntry}x{standardSelection}");
            }
            else if (form.AllKeys.Contains("trainingCourseSkip"))
            {
                Redirect(context, $"{GetUrlToSegment(currentUrl, 4)}0x0");
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
            var result = url.Segments.Take(segments + 1).Aggregate((x, y) => x + y);
            return result.EndsWith("/") ? result : result + "/";
        }
        private string GetFormValue(NameValueCollection form, string name, string defaultValue = "")
        {
            var value = form[name];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
        private bool PaybillIsEligibleForLevy(string paybillEntry)
        {
            int paybill;
            if (int.TryParse(paybillEntry, out paybill))
            {
                return paybill >= 3000000;
            }
            return false;
        }
    }
}