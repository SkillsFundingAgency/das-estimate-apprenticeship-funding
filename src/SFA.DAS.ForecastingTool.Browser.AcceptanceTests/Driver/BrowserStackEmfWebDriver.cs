using OpenQA.Selenium;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;

namespace SFA.DAS.ForecastingTool.Browser.AcceptanceTests.Driver
{
    public sealed class BrowserStackEmfWebDriver : EmfWebDriver
    {
        public BrowserStackEmfWebDriver(string uri, ICapabilities desiredCapability)
            : base(uri, desiredCapability)
        {

        }

    }
}

