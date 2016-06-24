using FluentAutomation;
using System;
using System.Linq.Expressions;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.PageObjects
{
    public class PayrollPage : PageObject<PayrollPage>
    {
        private const string NextButton = "input.button";
        private const string EnglishPercentageField = "input.form-control-1-3";

        public PayrollPage(FluentTest test) : base(test)
        {
        }

        public ApprenticesAndTrainingPage EnterLowPayroll()
        {
            I.Enter("10000").In(EnglishPercentageField);
            I.Click(NextButton);

            return Switch<ApprenticesAndTrainingPage>();
        }

        public PageObject EnterPayroll(string payroll)
        {
            I.WaitUntil(() => I.Assert.Exists(EnglishPercentageField));

            I.Enter(payroll).In(EnglishPercentageField);

            I.Click(NextButton);

            I.Wait(new TimeSpan(0, 0, 0, 0, 500));

            if (Convert.ToInt32(payroll) >= 3000000)
            {
                return Switch<EnglishPercentagePage>();
            }

            return Switch<ApprenticesAndTrainingPage>();
        }
        
    }
}
