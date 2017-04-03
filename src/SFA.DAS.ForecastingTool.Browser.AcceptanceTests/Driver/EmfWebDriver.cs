using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Extensions;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    public class EmfWebDriver : IEmfWebDriver
    {
        private readonly int _defaultTimeOutinSec;

        public RemoteWebDriver webDriver { get; set; }

        public EmfWebDriver(RemoteWebDriver remoteWebDriver, int timeout)
        {
            _defaultTimeOutinSec = timeout;
            webDriver = remoteWebDriver;
            ManageWebDriver();
        }
        public EmfWebDriver(string uri, ICapabilities desiredCapability)
        {
            _defaultTimeOutinSec = 10;
            webDriver = new RemoteWebDriver(new Uri(uri), desiredCapability);
            ManageWebDriver();
        }

        private void ManageWebDriver()
        {
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_defaultTimeOutinSec);
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(_defaultTimeOutinSec);
            webDriver.Manage().Window.Maximize();
        }

        public string BrowserName
        {
            get { return webDriver.Capabilities.BrowserName; }
        }

        public void GoToURL(string url)
        {
            webDriver.Navigate().GoToUrl(url);
        }

        public void Click(IWebElement webelement)
        {
            WaitforElementTobeDisplayedAndEnabled(webelement);
            webelement.Click();
        }


        public void TakeScreenshot(string filename)
        {
            try
            {
                var shot = webDriver.TakeScreenshot();
                shot.SaveAsFile(filename, ScreenshotImageFormat.Jpeg);
            }
            catch (Exception screenshotException)
            {
                Console.WriteLine(string.Format("'{0}' occured in TakeScreenshot", screenshotException.Message));
            }
        }

        public void WaitUntilDocIsReady()
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_defaultTimeOutinSec));
            wait.Until((webdriver) =>
               (webdriver as IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete")
            );
            return;
        }

        public void WaitforElementTobeDisplayedAndEnabled(IWebElement webelement)
        {
            var webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_defaultTimeOutinSec));

            webDriverWait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            webDriverWait.Until((webDriver) => ((webelement.Displayed && webelement.Enabled) == true));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    webDriver.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EMFWebDriver() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
