using TechTalk.SpecFlow;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Pages;
using NUnit.Framework;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Steps
{
    [Binding]
    public class LevyEmployerSteps
    {
        private readonly ScenarioContext scenarioContext;

        public LevyEmployerSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }
        
        [Given(@"My Annual Payroll is (\d+)")]
        public void GivenMyAnnualPayrollIs(int payroll)
        {
            var driver = scenarioContext.ScenarioContainer.Resolve<IEmfWebDriver>();
            var emfuri = scenarioContext.ScenarioContainer.Resolve<EmfUri>();
            EmfPage emfPage = new EmfPage(driver);
            PayrollPage payrollPage = emfPage.StartMyForecast(emfuri.MainUrl);
            if (payroll >= 3000000)
                scenarioContext.ScenarioContainer.RegisterInstanceAs(payrollPage.EnterPayrollAndReturnPercentagePage(payroll));
            else
                scenarioContext.ScenarioContainer.RegisterInstanceAs(payrollPage.EnterPayrollAndReturnChooseAppPage(payroll));
        }

        [Given(@"Roughly (.*) percentage of my employees live in england")]
        public void GivenRoughlyPercentageOfMyEmployeesLiveInEngland(string englandPercentage)
        {
            var englandPercentagePage = scenarioContext.ScenarioContainer.Resolve<EnglandPercentagePage>();

            ChooseApprenticeshipPage chooseApprenticeshipPage = englandPercentagePage.EnterEnglandPercentage(englandPercentage);

            scenarioContext.ScenarioContainer.RegisterInstanceAs(chooseApprenticeshipPage);
        }
        
        [When(@"I Skip apprenticeship selection")]
        public void WhenISkipApprenticeshipSelection()
        {
            var chooseApprenticeshipPage = scenarioContext.ScenarioContainer.Resolve<ChooseApprenticeshipPage>();
            EstimatedFundingPage estimatedFundingPage = chooseApprenticeshipPage.SkipThisStep();
            scenarioContext.ScenarioContainer.RegisterInstanceAs(estimatedFundingPage);
        }
        [When(@"I add (\d+) apprenticeship for (\d+) months starts on (\d+)/(\d+)")]
        public void WhenIAddApprenticeshipForMonthsStartsOn(int number, int duration, int startMonth,int startYear)
        {
            var chooseApprenticeshipPage = scenarioContext.ScenarioContainer.Resolve<ChooseApprenticeshipPage>();
            chooseApprenticeshipPage.ChooseApprenticeship(number, duration, startMonth, startYear);
        }
        
        [Then(@"My Annual and Monthly Levy Credit are shown")]
        public void ThenMyAnnualAndMonthlyLevyCreditAreShown()
        {
            var estimatedFundingPage = scenarioContext.ScenarioContainer.Resolve<EstimatedFundingPage>();
            Assert.IsTrue(estimatedFundingPage.IsYearlyLevyCreditDisplayed(), "Yearly Credit Limit is not displayed");
            Assert.IsTrue(estimatedFundingPage.IsMonthlyLevyCreditDisplayed(), "Monthly Credit Limit is not displayed");
        }
        [Then(@"My Annual and Monthly Levy Credit are not shown")]
        public void ThenMyAnnualAndMonthlyLevyCreditAreNotShown()
        {
            var estimatedFundingPage = scenarioContext.ScenarioContainer.Resolve<EstimatedFundingPage>();
            Assert.IsFalse(estimatedFundingPage.IsYearlyLevyCreditDisplayed(), "Yearly Credit Limit is displayed");
            Assert.IsFalse(estimatedFundingPage.IsMonthlyLevyCreditDisplayed(), "Monthly Credit Limit is displayed");
        }

        [Then(@"I should see how this is worked out")]
        public void ThenIShouldSeeHowThisIsWorkedOut()
        {
            var estimatedFundingPage = scenarioContext.ScenarioContainer.Resolve<EstimatedFundingPage>();
            Assert.IsTrue(estimatedFundingPage.IsHowthisisworkedoutDisplayed(), "How this is worked out details are not displayed");
        }

        [Then(@"I should see that i wont have to pay levy")]
        public void ThenIShouldSeeThatIWontHaveToPayLevy()
        {
            var estimatedFundingPage = scenarioContext.ScenarioContainer.Resolve<EstimatedFundingPage>();
            Assert.IsTrue(estimatedFundingPage.IsIWontPayLevyDisplayed(), "I won't pay levy details are not displayed");
        }

        [Then(@"I should see my co-investment details")]
        public void ThenIShouldSeeMyCo_InvestmentDetails()
        {
            var estimatedFundingPage = scenarioContext.ScenarioContainer.Resolve<EstimatedFundingPage>();
            Assert.IsTrue(estimatedFundingPage.IsCoInvestmentDetailsDisplayed(), "Co-Investment detail are not displayed");
        }

    }
}
