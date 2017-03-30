using OpenQA.Selenium.Support.PageObjects;
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
            Driver.WaitUntilDocIsReady();
            PageFactory.InitElements(Driver.WebDriver, this);
        }
        internal bool AreWeOnRightPage()
        {
            return string.IsNullOrEmpty(heading) ? false : Regex.Match(Driver.WebDriver.PageSource, heading).Success;
        }
        internal bool AreWeOnRightPage(string actual)
        {
            return string.IsNullOrEmpty(heading) ? false : Regex.Match(actual, heading).Success;
        }
    }
}
