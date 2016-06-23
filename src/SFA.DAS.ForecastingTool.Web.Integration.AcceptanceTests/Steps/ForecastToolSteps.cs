using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAutomation;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects;
using TechTalk.SpecFlow;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.Steps
{
    [Binding]
    public class ForecastToolSteps : FluentTest
    {
        private ResultsPage _scenarioContext;

        [BeforeScenario]
        public void Arrange()
        {
            SeleniumWebDriver.Bootstrap(SeleniumWebDriver.Browser.Chrome);

        }

        [Given(@"I am a non levy payer")]
        public void GivenIAmANonLevyPayer()
        {
            _scenarioContext = new WelcomePage(this)
                            .Go()
                            .GotToPayrollPage()
                            .EnterLowPayroll()
                            .IAddNoApprentices();
            
        }


        [Given(@"I a have a payroll of (.*)")]
        public void GivenIAHaveAPayrollOf(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I view the results page")]
        public void WhenIViewTheResultsPage()
        {
            Assert.AreEqual("",_scenarioContext.Url);
        }

        [Then(@"my english fraction is (.*)")]
        public void ThenMyEnglishFractionIs(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"I am not shown a results grid")]
        public void ThenIAmNotShownAResultsGrid()
        {
            I.Expect.Not.Exists(".resultstable");
        }


    }
}
