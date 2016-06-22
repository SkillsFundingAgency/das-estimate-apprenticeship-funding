using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.EuCookieMessage
{
    public static class EuCookieMessageManager
    {
        private const string CookieName = "seen_cookie_message";
        private const string MessageHtml = "<div id=\"global-cookie-message\" style=\"display: block;\"><p>GOV.UK uses cookies to make the site simpler. <a href=\"https://www.gov.uk/help/cookies\">Find out more about cookies</a></p></div>";

        public static string GetHtml()
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                throw new ArgumentException("HttpContext is not available");
            }

            if (HasCookie(context))
            {
                return string.Empty;
            }

            AddSeenCookieMessageCookie(context);
            return MessageHtml;
        }

        private static bool HasCookie(HttpContext context)
        {
            return context.Request.Cookies.AllKeys.Any(k => k == CookieName);
        }
        private static void AddSeenCookieMessageCookie(HttpContext context)
        {
            var cookie = new HttpCookie(CookieName)
            {
                Value = "true",
                Secure = context.Request.Url.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase),
                Expires = DateTime.Today.AddYears(1)
            };
            context.Request.Cookies.Add(cookie);
        }
    }
}