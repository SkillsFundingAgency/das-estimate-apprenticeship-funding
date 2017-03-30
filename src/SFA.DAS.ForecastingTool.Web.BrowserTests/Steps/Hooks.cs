using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext scenarioContext;
        private static string _url;

        public Hooks(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext", "this error raised when Hooks class constructor is invoked.");
            this.scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _url = ResolveAppConfig.GetSiteUrl();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var emfWebDriver = new PhantomJSEmfWebDriver();
            scenarioContext.ScenarioContainer.RegisterInstanceAs<IEmfWebDriver>(emfWebDriver);
            scenarioContext.ScenarioContainer.RegisterInstanceAs(new EmfUri { MainUrl = _url });
            emfWebDriver.WebDriver.Navigate().GoToUrl(_url);
        }

        [AfterScenario(Order = 2)]
        public void DisposeWebDriver()
        {
            var webDriver = scenarioContext.ScenarioContainer.Resolve<IEmfWebDriver>();

            webDriver?.Dispose();
        }

        [AfterScenario(Order =1)]
        public void TakescreenShotOnFailure()
        {
            if (scenarioContext.TestError !=null)
            {
                var webDriver = scenarioContext.ScenarioContainer.Resolve<IEmfWebDriver>();
                    webDriver.TakeScreenshot(string.Format("{0}{3}-{1}-{2}.jpeg", "C:\\",
                       scenarioContext.ScenarioInfo.Title.Replace(" ", ""),
                       DateTime.Now.ToString("ddMMyyyyHHmm"),
                       webDriver.BrowserName));

             }
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
