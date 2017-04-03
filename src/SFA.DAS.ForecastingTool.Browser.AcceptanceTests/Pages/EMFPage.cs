
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Pages
{
    public sealed class EmfPage : Base
    {
        public EmfPage(IEmfWebDriver driver) : base(driver)
        {
            heading = "Estimate my apprenticeship funding";
            if (AreWeOnRightPage(btnStart) == false) throw new IllegalStateException(string.Format("This is not the '{0}' page", heading));
        }
        
        [FindsBy(How = How.CssSelector, Using = ".button-start")]
        [CacheLookup]
        private IWebElement btnStart { get; set; }

        public PayrollPage StartMyForecast(string url)
        {
            Driver.Click(btnStart);
            return new PayrollPage(Driver);
        }
    }

}