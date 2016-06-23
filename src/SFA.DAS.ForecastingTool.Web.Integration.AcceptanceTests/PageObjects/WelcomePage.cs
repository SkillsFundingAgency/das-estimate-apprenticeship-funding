using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            I.Click(StartButton);

            return Switch<PayrollPage>();
        }
    }
}
