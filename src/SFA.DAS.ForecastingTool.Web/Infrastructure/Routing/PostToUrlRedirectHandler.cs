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
                decimal paybill;
                if (decimal.TryParse(paybillEntry, out paybill))
                {
                    paybillEntry = Math.Floor(paybill).ToString();
                }
                if (PaybillIsEligibleForLevy(paybillEntry))
                {
                    Redirect(context, $"{currentUrl.GetUrlToSegment(1)}{EncodeFormEntryForUrl(paybillEntry)}/");
                }
                else
                {
                    Redirect(context, $"{currentUrl.GetUrlToSegment(1)}{EncodeFormEntryForUrl(paybillEntry)}/100/");
                }
            }
            else if (form.AllKeys.Contains("englishFractionSubmit"))
            {
                var englishFractionEntry = GetFormValue(form, "englishFraction", "0");
                decimal englishFraction;
                if (decimal.TryParse(englishFractionEntry, out englishFraction))
                {
                    englishFractionEntry = Math.Floor(englishFraction).ToString();
                }
                Redirect(context, $"{currentUrl.GetUrlToSegment(2)}{EncodeFormEntryForUrl(englishFractionEntry)}/");
            }
            else if (form.AllKeys.Contains("englishFractionSkip"))
            {
                Redirect(context, $"{currentUrl.GetUrlToSegment(2)}100/");
            }
            else if (form.AllKeys.Contains("trainingCourseSubmit"))
            {
                var cohorstSegment = GetCohortsUrlSegment(form);
                if (string.IsNullOrEmpty(cohorstSegment))
                {
                    cohorstSegment = "noselection";
                }
                Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{EncodeFormEntryForUrl(cohorstSegment)}/12/");
            }
            else if (form.AllKeys.Contains("trainingCourseSkip"))
            {
                Redirect(context, $"{currentUrl.GetUrlToSegment(3)}0x0/12/");
            }
            else if (form.AllKeys.Contains("trainingCourseAdd"))
            {
                var cohorstSegment = GetCohortsUrlSegment(form);
                Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{EncodeFormEntryForUrl(cohorstSegment)}/");
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

        private string EncodeFormEntryForUrl(string entry)
        {
            var encoded = HttpUtility.UrlEncode(entry);
            while (encoded.EndsWith("."))
            {
                encoded = encoded.Substring(0, encoded.Length - 1);
            }
            return encoded;
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

        private string GetCohortsUrlSegment(NameValueCollection form)
        {
            var cohortsEntry = GetFormValue(form, "cohorts", "0").Split(',');
            var standardSelection = GetFormValue(form, "standard", "0").Split(',');
            var startDateEntry = GetFormValue(form, "startDate", "2017-04-01").Split(',');

            var segmentbuilder = new StringBuilder();

            for (var i = 0; i < standardSelection.Length; i ++)
            {
                if (standardSelection[i] == "noselection")
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