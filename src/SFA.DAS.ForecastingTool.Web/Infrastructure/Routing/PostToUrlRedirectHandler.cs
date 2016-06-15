using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

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
                    Redirect(context, $"{currentUrl.GetUrlToSegment(1)}{paybillEntry}");
                }
                else
                {
                    Redirect(context, $"{currentUrl.GetUrlToSegment(1)}{paybillEntry}/100");
                }
            }
            else if (form.AllKeys.Contains("englishFractionSubmit"))
            {
                var englishFractionEntry = GetFormValue(form, "englishFraction", "0");
                Redirect(context, $"{currentUrl.GetUrlToSegment(2)}{englishFractionEntry}");
            }
            else if (form.AllKeys.Contains("englishFractionSkip"))
            {
                Redirect(context, $"{currentUrl.GetUrlToSegment(2)}100");
            }
            else if (form.AllKeys.Contains("trainingCourseSubmit"))
            {
                var cohorstSegment = GetCohortsUrlSegment(form, true);
                Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{cohorstSegment}/12");
            }
            else if (form.AllKeys.Contains("trainingCourseSkip"))
            {
                Redirect(context, $"{currentUrl.GetUrlToSegment(4)}0x0/12");
            }
            else if (form.AllKeys.Contains("trainingCourseAdd"))
            {
                var cohorstSegment = GetCohortsUrlSegment(form, false);
                Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{cohorstSegment}");
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
        
        private string GetFormValue(NameValueCollection form, string name, string defaultValue = "")
        {
            var value = form[name];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
        private bool PaybillIsEligibleForLevy(string paybillEntry)
        {
            long paybill;
            if (long.TryParse(paybillEntry, out paybill))
            {
                return paybill >= 3000000;
            }
            return false;
        }

        private string GetCohortsUrlSegment(NameValueCollection form, bool includeUnslectedRows)
        {
            var cohortsEntry = GetFormValue(form, "cohorts", "0").Split(',');
            var standardSelection = GetFormValue(form, "standard", "0").Split(',');
            var startDateEntry = GetFormValue(form, "startDate", "2017-04-01").Split(',');

            var segmentbuilder = new StringBuilder();

            for (var i = 0; i < standardSelection.Length; i ++)
            {
                if (!includeUnslectedRows && standardSelection[i] == "noselection")
                {
                    continue;
                }
                if (i > 0)
                {
                    segmentbuilder.Append("_");
                }
                segmentbuilder.Append($"{cohortsEntry[i]}x{standardSelection[i]}-{startDateEntry[i]}");
            }

            return segmentbuilder.ToString();
        }
    }
}