using System.Web.Mvc;

namespace SFA.DAS.ForecastingTool.Web
{
    public static class HtmlHelperExtensions
    {
        public static string FinancialText(this HtmlHelper htmlHelper, decimal amount)
        {
            if (amount == 0)
            {
                return "-";
            }
            return amount.ToString("C");
        }
    }
}