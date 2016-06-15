using System;
using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public class ForecastCalculator : IForecastCalculator
    {
        private readonly IStandardsRepository _standardsRepository;
        private readonly IConfigurationProvider _configurationProvider;


        public ForecastCalculator(IStandardsRepository standardsRepository, IConfigurationProvider configurationProvider)
        {
            _standardsRepository = standardsRepository;
            _configurationProvider = configurationProvider;
        }



        public async Task<ForecastResult> ForecastAsync(int paybill, int englishFraction, StandardModel[] myStandards, int duration)
        {
            var levyPaid = (paybill * _configurationProvider.LevyPercentage) - _configurationProvider.LevyAllowance;
            if (levyPaid < 0) // Non-levy payer
            {
                levyPaid = 0;
            }

            var decimalEnglishFraction = englishFraction / 100m;
            var fundingReceived = (levyPaid * decimalEnglishFraction) * _configurationProvider.LevyTopupPercentage;

            var breakdown = await CalculateBreakdown(myStandards, fundingReceived, duration);

            return new ForecastResult
            {
                FundingReceived = fundingReceived,
                LevyPaid = levyPaid,
                UserFriendlyTopupPercentage = (int)Math.Round((_configurationProvider.LevyTopupPercentage - 1) * 100, 0),
                Breakdown = breakdown
            };
        }



        private async Task<MonthlyCashflow[]> CalculateBreakdown(StandardModel[] cohorts, decimal fundingReceived, int duration)
        {
            var standards = await ExpandCohortModels(cohorts);
            var startDate = new DateTime(2017, 4, 1);

            var monthlyFunding = fundingReceived / 12m;
            var sunsetLimit = monthlyFunding * 18;

            var rollingBalance = 0m;
            var months = new MonthlyCashflow[duration];
            for (var i = 0; i < duration; i++)
            {
                var trainingCostForMonth = 0m;
                var monthDate = startDate.AddMonths(i);
                var sunsetFunds = 0m;
                var fundsReceived = monthDate == startDate ? 0m : monthlyFunding;

                foreach (var standard in standards)
                {
                    var trainingStartDate = new DateTime(standard.StartDate.Year, standard.StartDate.Month, 1);
                    var trainingEndDate = trainingStartDate.AddMonths(standard.Standard.Duration - 1);
                    var trainingHasStarted = monthDate.CompareTo(trainingStartDate) >= 0;
                    var trainingHasFinished = monthDate.CompareTo(trainingEndDate) > 0;
                    trainingCostForMonth += trainingHasStarted && !trainingHasFinished ? standard.MonthlyTrainingFraction : 0;
                    if (monthDate.CompareTo(trainingEndDate) == 0)
                    {
                        trainingCostForMonth += standard.FinalPaymentAmount;
                    }
                }

                rollingBalance += fundsReceived - trainingCostForMonth;
                if (rollingBalance > sunsetLimit)
                {
                    sunsetFunds = rollingBalance - sunsetLimit;
                    rollingBalance = sunsetLimit;
                }

                months[i] = new MonthlyCashflow
                {
                    Date = monthDate,
                    LevyIn = Math.Round(fundsReceived, 2),
                    TrainingOut = Math.Round(trainingCostForMonth, 2),
                    Balance = rollingBalance < 0 ? 0 : Math.Round(rollingBalance, 2),
                    SunsetFunds = Math.Round(sunsetFunds, 2),
                    CoPayment = rollingBalance < 0 ? Math.Round((rollingBalance * -1) * _configurationProvider.CopaymentPercentage, 2) : 0
                };

                // Have we just balanced the account?
                if (rollingBalance < 0)
                {
                    rollingBalance = 0;
                }
            }
            return months;
        }

        private async Task<BreakdownStandard[]> ExpandCohortModels(StandardModel[] cohorts)
        {
            var standards = new BreakdownStandard[cohorts.Length];
            for (var i = 0; i < cohorts.Length; i++)
            {
                standards[i] = new BreakdownStandard(await _standardsRepository.GetByCodeAsync(cohorts[i].Code),
                    cohorts[i].Qty, cohorts[i].StartDate, _configurationProvider.FinalTrainingPaymentPercentage);
            }
            return standards;
        }




        private class BreakdownStandard
        {
            public BreakdownStandard(Standard standard, int qty, DateTime startDate, decimal finalPaymentPercentage)
            {
                Standard = standard;
                Qty = qty;
                StartDate = startDate;

                var totalCost = standard.Price * qty;
                FinalPaymentAmount = totalCost * finalPaymentPercentage;
                MonthlyTrainingFraction = (totalCost - FinalPaymentAmount) / standard.Duration;
            }


            public Standard Standard { get; }
            public int Qty { get; }
            public DateTime StartDate { get; }
            public decimal MonthlyTrainingFraction { get; }
            public decimal FinalPaymentAmount { get; set; }
        }
    }
}