using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using NLog;
using SFA.DAS.ForecastingTool.Web.Extensions;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class PostToUrlRedirectHandler : IHttpHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly ActionHandlerMapping[] _actionHandlers;

        public PostToUrlRedirectHandler()
        {
            _actionHandlers = new[]
            {
                new ActionHandlerMapping {ActionKey = "paybillSubmit", Handler = PaybillSubmit},
                new ActionHandlerMapping {ActionKey = "englishFractionSubmit", Handler = EnglishFractionSubmit},
                new ActionHandlerMapping {ActionKey = "englishFractionSkip", Handler = EnglishFractionSkip},
                new ActionHandlerMapping {ActionKey = "trainingCourseSubmit", Handler = TrainingCourseSubmit},
                new ActionHandlerMapping {ActionKey = "trainingCourseSkip", Handler = TrainingCourseSkip},
                new ActionHandlerMapping {ActionKey = "trainingCourseAdd", Handler = TrainingCourseAdd},
                new ActionHandlerMapping {ActionKey = "trainingCourseDelete", Handler = TrainingCourseDelete}
            };
        }


        public void ProcessRequest(HttpContext context)
        {
            var form = context.Request.Form;
            var currentUrl = context.Request.Url;

            var handler = GetHandler(form);
            if (handler == null)
            {
                Logger.Warn($"Invalid form post received to {currentUrl} from {context.Request.UserHostAddress}");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            try
            {
                AntiForgery.Validate();
            }
            catch (HttpAntiForgeryException ex)
            {
                Logger.Warn(ex, $"Request received to {currentUrl} from {context.Request.UserHostAddress} with invalid anti-forgery token.");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            handler.Handler.Invoke(context, form, currentUrl);
        }

        public bool IsReusable { get; } = false;


        private void PaybillSubmit(HttpContext context, NameValueCollection form, Uri currentUrl)
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
                Redirect(context, $"{currentUrl.GetUrlToSegment(1)}{EncodeFormEntryForUrl(paybillEntry)}/NA/");
            }
        }
        private void EnglishFractionSubmit(HttpContext context, NameValueCollection form, Uri currentUrl)
        {
            var englishFractionEntry = GetFormValue(form, "englishFraction", "0");
            decimal englishFraction;
            if (decimal.TryParse(englishFractionEntry, out englishFraction))
            {
                englishFractionEntry = Math.Floor(englishFraction).ToString();
            }
            Redirect(context, $"{currentUrl.GetUrlToSegment(2)}{EncodeFormEntryForUrl(englishFractionEntry)}/");
        }
        private void EnglishFractionSkip(HttpContext context, NameValueCollection form, Uri currentUrl)
        {
            Redirect(context, $"{currentUrl.GetUrlToSegment(2)}100/");
        }
        private void TrainingCourseSubmit(HttpContext context, NameValueCollection form, Uri currentUrl)
        {
            var cohorstSegment = GetCohortsUrlSegment(form);
            if (string.IsNullOrEmpty(cohorstSegment))
            {
                cohorstSegment = "noselection";
            }
            Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{EncodeFormEntryForUrl(cohorstSegment)}/12/");
        }
        private void TrainingCourseSkip(HttpContext context, NameValueCollection form, Uri currentUrl)
        {
            Redirect(context, $"{currentUrl.GetUrlToSegment(3)}0x0/12/");
        }
        private void TrainingCourseAdd(HttpContext context, NameValueCollection form, Uri currentUrl)
        {
            var cohorstSegment = GetCohortsUrlSegment(form);
            Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{EncodeFormEntryForUrl(cohorstSegment)}/#next-apprenticeship");
        }
        private void TrainingCourseDelete(HttpContext context, NameValueCollection form, Uri currentUrl)
        {
            var cohortToDelete = form.AllKeys.First(k => k.StartsWith("trainingCourseDelete")).Substring(20);
            var originalCohorstSegment = GetCohortsUrlSegment(form);
            var fixedCohortSegment = originalCohorstSegment.Replace(cohortToDelete, "").Replace("__", "_");
            if (fixedCohortSegment.StartsWith("_"))
            {
                fixedCohortSegment = fixedCohortSegment.Substring(1);
            }
            Redirect(context, $"{currentUrl.GetUrlToSegment(3)}{EncodeFormEntryForUrl(fixedCohortSegment)}/");
        }


        private ActionHandlerMapping GetHandler(NameValueCollection form)
        {
            foreach (var handler in _actionHandlers)
            {
                if (form.AllKeys.Any(k => k.StartsWith(handler.ActionKey)))
                {
                    return handler;
                }
            }
            return null;
        }
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
            var startDateMonthEntry = GetFormValue(form, "startDateMonth", "05").Split(',');
            var startDateYearEntry = GetFormValue(form, "startDateYear", "17").Split(',');

            var segmentbuilder = new StringBuilder();

            for (var i = 0; i < standardSelection.Length; i++)
            {
                if (standardSelection[i] == "noselection" && string.IsNullOrEmpty(cohortsEntry[i]) && string.IsNullOrEmpty(startDateMonthEntry[i]) && string.IsNullOrEmpty(startDateYearEntry[i]))
                {
                    continue;
                }
                if (i > 0)
                {
                    segmentbuilder.Append("_");
                }

                segmentbuilder.Append($"{cohortsEntry[i]}x{standardSelection[i]}-{startDateMonthEntry[i].PadLeft(2, '0')}{startDateYearEntry[i]}");
            }

            return segmentbuilder.ToString();
        }
    }

    internal class ActionHandlerMapping
    {
        public string ActionKey { get; set; }
        public Action<HttpContext, NameValueCollection, Uri> Handler { get; set; }
    }
}