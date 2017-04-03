using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using SFA.DAS.ForecastingTool.Browser.AcceptanceTests.Driver;
using System.IO;
using System.Reflection;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext scenarioContext;
        private static string _url;
        public static string _RemoteDriverUri;

        private string _assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public Hooks(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext", "this error raised when Hooks class constructor is invoked.");
            this.scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _url = ResolveAppConfig.GetSiteUrl();
            _RemoteDriverUri = ResolveAppConfig.GetBrowserStackUri();
        }

        //[BeforeScenario(Order = 1)]
        //public void ConnecttoBrowserStack()
        //{
        //    var capdictionary = new Dictionary<string, string>();
        //    capdictionary.Add("browser", "ie");
        //    capdictionary.Add("browser_version", "11");
        //    capdictionary.Add("os", "WINDOWS");
        //    capdictionary.Add("os_version", "10");
        //    capdictionary.Add("build", "TestIE1.0");
        //    capdictionary.Add("project", "EMF-Dev url");
        //    capdictionary.Add("name", scenarioContext.ScenarioInfo.Title.ToString());

        //    var desiredCap = DesiredCapabilities.Chrome();

        //    foreach(var capability in capdictionary)
        //    {
        //        desiredCap.SetCapability(capability.Key, capability.Value);
        //    }

        //    scenarioContext.ScenarioContainer.RegisterInstanceAs(new BrowserStackURi { RemoteDriverUri = _RemoteDriverUri });
        //    scenarioContext.ScenarioContainer.RegisterInstanceAs<ICapabilities>(desiredCap);
        //}

        [BeforeScenario(Order = 2)]
        public void BeforeScenario()
        {

            //var emfWebDriver = new BrowserStackEmfWebDriver(_RemoteDriverUri, scenarioContext.ScenarioContainer.Resolve<ICapabilities>());
            var emfWebDriver = new PhantomJSEmfWebDriver(_assemblyFolder);
            scenarioContext.ScenarioContainer.RegisterInstanceAs<IEmfWebDriver>(emfWebDriver);
            scenarioContext.ScenarioContainer.RegisterInstanceAs(new EmfUri { MainUrl = _url });
            emfWebDriver.webDriver.Navigate().GoToUrl(_url);
        }

        [AfterScenario(Order = 1)]
        public void TakescreenShotOnFailure()
        {
            if (scenarioContext.TestError !=null)
            {
                var webDriver = scenarioContext.ScenarioContainer.Resolve<IEmfWebDriver>();
                
                var fileName = string.Format("{2}-{0}-{1}.jpg", 
                               scenarioContext.ScenarioInfo.Title.Replace(" ", ""),
                               DateTime.Now.ToString("ddMMyyyyHHmm"),
                               webDriver.BrowserName);
                string screenshotFileName = Path.Combine(_assemblyFolder, fileName);
                Console.WriteLine(screenshotFileName);
                    webDriver.TakeScreenshot(screenshotFileName);
             }
        }

        [AfterScenario(Order = 2)]
        public void DisposeWebDriver()
        {
            var webDriver = scenarioContext.ScenarioContainer.Resolve<IEmfWebDriver>();
            webDriver?.Dispose();
        }


        [AfterFeature]
        public static void AfterFeature()
        {
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
        }
    }
}
