using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Pages
{
    public class EstimatedFundingPage : Base
    {
        public EstimatedFundingPage(IEmfWebDriver driver) : base(driver)
        {
            heading = "Your estimated apprenticeship funding";
            if (AreWeOnRightPage() == false) throw new IllegalStateException(string.Format("This is not the '{0}' page", heading));
        }

        [FindsBy(How = How.Id, Using = "content")]
        private IWebElement content { get; set; }

        [FindsBy(How = How.CssSelector, Using = "table.results-table")]
        private IWebElement resultTable { get; set; }

        [FindsBy(How = How.CssSelector, Using = "table tbody td")]
        private IList<IWebElement> tableContent { get; set; }

        public bool IsYearlyLevyCreditDisplayed()
        {
            return Regex.Match(content.Text, 
                "You’ll receive £.+ of levy credit per year. Any unused credit will expire .+ months after it arrives in your account.").Success;
        }

        public bool IsMonthlyLevyCreditDisplayed()
        {
            return Regex.Match(tableContent.ToList().Take(2).Last().Text, "£.+").Success;
        }

        public bool IsHowthisisworkedoutDisplayed()
        {
            return Regex.Match(content.Text, "How this is worked out").Success;
        }

        public bool IsCoInvestmentDetailsDisplayed()
        {
            return Regex.Match(tableContent.ToList().Take(4).Last().Text, "£.+").Success &&
                Regex.Match(tableContent.ToList().Take(5).Last().Text, "£.+").Success;
        }

        public bool IsIWontPayLevyDisplayed()
        {
            return Regex.Match(content.Text, "Your payroll is less than £.+ million so you won't have to pay the levy.").Success;
        }
    }
}