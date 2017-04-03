
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Pages
{
    public class EnglandPercentagePage : Base
    {
        public EnglandPercentagePage(IEmfWebDriver driver) : base(driver)
        {
            heading = "Employees living in England";
            if (AreWeOnRightPage(btnSaveAndContinue) == false) throw new IllegalStateException(string.Format("This is not the '{0}' page", heading));
        }

        [FindsBy(How = How.Name, Using = "englishFraction")]
        [CacheLookup]
        private IWebElement txtPercentage { get; set; }

        [FindsBy(How = How.Name, Using = "englishFractionSubmit")]
        [CacheLookup]
        private IWebElement btnSaveAndContinue { get; set; }

        public ChooseApprenticeshipPage EnterEnglandPercentage(string percentage)
        {
            txtPercentage.Type(percentage);
            Driver.Click(btnSaveAndContinue);
            return new ChooseApprenticeshipPage(Driver);
        }
    }
}