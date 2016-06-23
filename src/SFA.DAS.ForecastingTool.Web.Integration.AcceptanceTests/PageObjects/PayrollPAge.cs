using FluentAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects
{
    public class PayrollPage : PageObject<PayrollPage>
    {
        public PayrollPage(FluentTest test) : base(test)
        {
        }

        public ApprenticesAndTrainingPage EnterLowPayroll()
        {
            I.Enter("10000").In("input.fomr-control-1-3");

            return Switch<ApprenticesAndTrainingPage>();
        }
    }
}
