using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Pages
{
    public class ChooseApprenticeshipPage : Base
    {
        public ChooseApprenticeshipPage(IEmfWebDriver driver) :base(driver)
        {
            heading = "Choose apprenticeships";
            if (AreWeOnRightPage(lstapprenticeship) == false) throw new IllegalStateException(string.Format("This is not the '{0}' page", heading));
        }

        [FindsBy(How = How.Name, Using = "trainingCourseSkip")]
        private IWebElement lnkSkipthisStep { get; set; }

        [FindsBy(How = How.CssSelector, Using = "input.button")]
        private IList<IWebElement> buttons { get; set; }

        [FindsBy(How = How.Id, Using = "standardNew")]
        private IWebElement lstapprenticeship { get; set; }

        [FindsBy(How = How.Id, Using = "cohortsNew")]
        private IWebElement txtnumber { get; set; }

        [FindsBy(How = How.Id, Using = "startDateMonth")]
        private IWebElement txtstartDateMonth { get; set; }

        [FindsBy(How = How.Id, Using = "startDateYear")]
        private IWebElement txtstartDateYear { get; set; }


        public EstimatedFundingPage SkipThisStep()
        {
            Driver.Click(lnkSkipthisStep);
            return new EstimatedFundingPage(Driver);
        }

        public void ChooseApprenticeship(int number, int duration, int startMonth, int StartYear)
        {
            SelectApprentiship(number, duration, startMonth, StartYear);

            Driver.Click(buttons.FirstOrDefault(x => x.GetAttribute("value") == "Save and continue"));
        }

        private void SelectApprentiship(int number, int duration, int startMonth, int StartYear)
        {
            Func<SelectElement> selectElement = () =>
            {
                return new SelectElement(lstapprenticeship);
            };

            List<IWebElement> optionList = selectElement().Options.ToList();

            IWebElement filteredOption = optionList.FirstOrDefault(x => x.GetAttribute("data-duration") == duration.ToString());

            selectElement().SelectByText(filteredOption.Text);

            txtnumber.Type(number.ToString());

            txtstartDateMonth.Type(startMonth.ToString());

            txtstartDateYear.Type(StartYear.ToString());
        }
    }
}