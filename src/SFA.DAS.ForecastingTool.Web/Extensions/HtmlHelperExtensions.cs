using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.Infrastructure.EuCookieMessage;

namespace SFA.DAS.ForecastingTool.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string FinancialText(this HtmlHelper htmlHelper, decimal amount)
        {
            if (amount == 0)
            {
                return "-";
            }
            return amount.ToString("C0");
        }

        public static MvcHtmlString CookieWarning(this HtmlHelper htmlHelper)
        {
            return new MvcHtmlString(EuCookieMessageManager.GetHtml());
        }
        
    }
}