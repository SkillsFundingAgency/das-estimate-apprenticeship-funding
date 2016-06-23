using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAutomation;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects
{
    public class ApprenticesAndTrainingPage : PageObject<ApprenticesAndTrainingPage>
    {
        public ApprenticesAndTrainingPage(FluentTest test) : base(test)
        {
        }

        public ResultsPage IAddNoApprentices()
        {
            I.Click("a[href='0x0/12']");

            return Switch<ResultsPage>();
        }

    }
}
