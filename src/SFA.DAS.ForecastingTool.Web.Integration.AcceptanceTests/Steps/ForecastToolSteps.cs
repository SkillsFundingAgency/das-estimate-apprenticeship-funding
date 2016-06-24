using System.Configuration;
using FluentAutomation;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects;
using TechTalk.SpecFlow;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.Steps
{
    [Binding , Scope(Tag="Automation")]
    public class ForecastToolSteps : FluentTest
    {
        private ResultsPage _scenarioContextResultsPage;
        private PageObject _scenarioContextEnglishFraction;
        

        [BeforeScenario]
        public void Arrange()
        {
            SeleniumWebDriver.Bootstrap(SeleniumWebDriver.Browser.Chrome);

        }

        [Given(@"I am a non levy payer")]
        public void GivenIAmANonLevyPayer()
        {
            _scenarioContextResultsPage = new WelcomePage(this)
                            .Go()
                            .GotToPayrollPage()
                            .EnterLowPayroll()
                            .IAddNoApprentices();
            
        }


        [Given(@"I have a payroll of (.*)")]
        public void GivenIAHaveAPayrollOf(string payroll)
        {
            ScenarioContext.Current["payroll"] = payroll;

            _scenarioContextEnglishFraction = new WelcomePage(this)
                .Go()
                .GotToPayrollPage()
                .EnterPayroll(payroll);

        }

        [When(@"I view the results page")]
        public void WhenIViewTheResultsPage()
        {
            _scenarioContextResultsPage.I.Assert.Url($"{ConfigurationManager.AppSettings["TestBaseUrl"]}forecast/10000/NA/0x0/12");    
        }

        [Then(@"my english fraction is (.*)")]
        public void ThenMyEnglishFractionIs(string englishFraction)
        {
            var payroll = ScenarioContext.Current["payroll"].ToString();
            if (englishFraction == "NA")
            {
                ((ApprenticesAndTrainingPage) _scenarioContextEnglishFraction).I.Assert.Url($"{ConfigurationManager.AppSettings["TestBaseUrl"]}forecast/{payroll}/NA/");
            }
            else
            {
                var englishPercentagePage = (EnglishPercentagePage)_scenarioContextEnglishFraction;
                var result = englishPercentagePage.EnterPercentageAndGotoNextPage(englishFraction);
                
                result.I.Assert.Url($"{ConfigurationManager.AppSettings["TestBaseUrl"]}forecast/{payroll}/{englishFraction}/");
            }
        }

        [Then(@"I am not shown a results grid")]
        public void ThenIAmNotShownAResultsGrid()
        {
            _scenarioContextResultsPage.I.Assert.Not.Exists(".results-table");
        }
        
    }
}
