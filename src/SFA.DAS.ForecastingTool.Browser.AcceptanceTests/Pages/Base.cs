using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;
using System.Text.RegularExpressions;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Pages
{

    public abstract class Base
    {
        public IEmfWebDriver Driver;
        public string heading { get; set; }
        public Base(IEmfWebDriver emfdriver)
        {
            Driver = emfdriver;
            PageFactory.InitElements(Driver.webDriver, this);
            Driver.WaitUntilDocIsReady();
        }

        internal bool AreWeOnRightPage(IWebElement webelement)
        {
            return WaitAndAssertOnPage(webelement);
        }

        private bool WaitAndAssertOnPage(IWebElement webelement)
        {
            try
            {
                Driver.WaitforElementTobeDisplayedAndEnabled(webelement);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
