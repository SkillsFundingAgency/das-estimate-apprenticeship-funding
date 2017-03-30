using OpenQA.Selenium.Chrome;
using System;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    public sealed class ChromeEmfWebDriver : EmfWebDriver
    {
        private static int _defaultTimeOutinSec = 10;

        public ChromeEmfWebDriver()
            : this(AppDomain.CurrentDomain.BaseDirectory)
        {

        }
        public ChromeEmfWebDriver(int pageNavigationTimeout)
            : this(AppDomain.CurrentDomain.BaseDirectory, pageNavigationTimeout)
        {
        }

        public ChromeEmfWebDriver(string filepath)
            : this(filepath, _defaultTimeOutinSec)
        {
        }

        public ChromeEmfWebDriver(string filepath, int pageNavigationTimeout)
            : base(new ChromeDriver(filepath, Options()), pageNavigationTimeout)
        {
            _defaultTimeOutinSec = pageNavigationTimeout;
        }

        private static ChromeOptions Options()
        {
            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.prompt_for_download", true);
            options.AddArguments("no-sandbox");
            return options;
        }
    }
}
