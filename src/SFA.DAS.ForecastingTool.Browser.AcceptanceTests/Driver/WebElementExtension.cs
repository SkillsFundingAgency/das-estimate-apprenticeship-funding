
using OpenQA.Selenium;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    public static class WebElementExtension
    {
        public static void Type(this IWebElement webelement, string text)
        {
            webelement.Clear();
            webelement.SendKeys(text);
        }
    }
}
