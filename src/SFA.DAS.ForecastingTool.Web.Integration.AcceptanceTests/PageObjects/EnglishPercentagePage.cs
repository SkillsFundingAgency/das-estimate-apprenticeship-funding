using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAutomation;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects
{
    public class EnglishPercentagePage : PageObject<EnglishPercentagePage>
    {
        private const string EnglishFractionInput = "input.form-control";
        private const string NextButton = "input.button";

        public EnglishPercentagePage(FluentTest test) : base(test)
        {
        }

        public ApprenticesAndTrainingPage GoToNextPage()
        {


            return Switch<ApprenticesAndTrainingPage>();
        }

        public ApprenticesAndTrainingPage EnterPercentageAndGotoNextPage(string englishFraction)
        {
            I.WaitUntil(() => I.Assert.Exists(EnglishFractionInput));

            I.Enter(englishFraction).In(EnglishFractionInput);
            I.Click(NextButton);

            return Switch<ApprenticesAndTrainingPage>();
        }
    }
}
