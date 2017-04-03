using OpenQA.Selenium.PhantomJS;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    public sealed class PhantomJSEmfWebDriver : EmfWebDriver
    {
        private static int _defaultTimeOutinSec = 10;

        public PhantomJSEmfWebDriver(string filepath)
            : this(filepath, _defaultTimeOutinSec)
        {
        }

        public PhantomJSEmfWebDriver(string filepath, int pageNavigationTimeout)
            : base(new PhantomJSDriver(filepath, Options()), pageNavigationTimeout)
        {
            _defaultTimeOutinSec = pageNavigationTimeout;
        }
        private static PhantomJSOptions Options()
        {
            var options = new PhantomJSOptions();
            return options;
        }
    }
}
