using FluentAutomation;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects
{
    public class ApprenticesAndTrainingPage : PageObject<ApprenticesAndTrainingPage>
    {
        private string SkipButton = "a[href='0x0/12']";

        public ApprenticesAndTrainingPage(FluentTest test) : base(test)
        {
        }

        public ResultsPage IAddNoApprentices()
        {
            I.WaitUntil(() => I.Assert.Exists(SkipButton));
            
            I.Click(SkipButton);

            return Switch<ResultsPage>();
        }

    }
}
