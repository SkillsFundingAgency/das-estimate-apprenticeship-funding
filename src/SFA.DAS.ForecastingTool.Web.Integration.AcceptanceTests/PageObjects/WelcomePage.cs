using FluentAutomation;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects
{
    public class WelcomePage : PageObject<WelcomePage>
    {
        private const string StartButton = "a.button-start";

        public WelcomePage(FluentTest test) : base(test)
        {
            Url = "http://localhost:6060";
            At = () => I.Expect.Exists(StartButton);
        }

        public PayrollPage GotToPayrollPage()
        {
            I.WaitUntil(() => I.Assert.Exists(StartButton));

            I.Click(StartButton);

            return Switch<PayrollPage>();
        }
    }
}
