
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SFA.DAS.ForecastingTool.Web.BrowserTests.Driver;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Pages
{
    public sealed class PayrollPage : Base
    {
        public PayrollPage(IEmfWebDriver driver) : base(driver)
        {
            heading = "Enter your organisation’s UK payroll";
            if (AreWeOnRightPage(btnSaveAndContinue) == false) throw new IllegalStateException(string.Format("This is not the '{0}' page", heading));
        }

        [FindsBy(How = How.Id, Using = "paybill")]
        [CacheLookup]
        private IWebElement txtpayroll { get; set; }

        [FindsBy(How = How.Name, Using = "paybillSubmit")]
        [CacheLookup]
        private IWebElement btnSaveAndContinue { get; set; }

        public EnglandPercentagePage EnterPayrollAndReturnPercentagePage(int payrollAmount)
        {
            EnterPayroll(payrollAmount);
            return new EnglandPercentagePage(Driver);
        }

        public ChooseApprenticeshipPage EnterPayrollAndReturnChooseAppPage(int payrollAmount)
        {
            EnterPayroll(payrollAmount);
            return new ChooseApprenticeshipPage(Driver);
        }

        private void EnterPayroll(int payrollAmount)
        {
            txtpayroll.Type(payrollAmount.ToString());
            Driver.Click(btnSaveAndContinue);
        }

    }
}