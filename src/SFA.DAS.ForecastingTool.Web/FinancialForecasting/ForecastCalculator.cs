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


        public async Task<ForecastResult> ForecastAsync(long paybill, int englishFraction)
        {
            var monthlyLevyPaid = Math.Floor(((paybill * _configurationProvider.LevyPercentage) - _configurationProvider.LevyAllowance) / 12);
            if (monthlyLevyPaid < 0) // Non-levy payer
            {
                monthlyLevyPaid = 0;
            }
            var levyPaid = monthlyLevyPaid * 12;

            var decimalEnglishFraction = englishFraction / 100m;
            var fundingReceived = Math.Ceiling((monthlyLevyPaid * decimalEnglishFraction) * _configurationProvider.LevyTopupPercentage) * 12;

            return new ForecastResult
            {
                MonthlyLevyPaid = monthlyLevyPaid,
                FundingReceived = fundingReceived,
                LevyPaid = levyPaid,
                UserFriendlyTopupPercentage = (int)Math.Round((_configurationProvider.LevyTopupPercentage - 1) * 100, 0)
            };
        }
        public async Task<DetailedForecastResult> DetailedForecastAsync(long paybill, int englishFraction, CohortModel[] cohorts, int duration)
        {
            var forecastResult = await ForecastAsync(paybill, englishFraction);

            var breakdown = await CalculateBreakdown(cohorts, forecastResult.FundingReceived, duration);

            return new DetailedForecastResult
            {
                MonthlyLevyPaid = forecastResult.MonthlyLevyPaid,
                FundingReceived = forecastResult.FundingReceived,
                LevyPaid = forecastResult.LevyPaid,
                UserFriendlyTopupPercentage = (int)Math.Round((_configurationProvider.LevyTopupPercentage - 1) * 100, 0),
                Breakdown = breakdown
            };
        }



        private async Task<MonthlyCashflow[]> CalculateBreakdown(CohortModel[] cohorts, decimal fundingReceived, int duration)
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
                var finalPaymentMade = false;

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
                        finalPaymentMade = true;
                    }
                }

                rollingBalance += fundsReceived - trainingCostForMonth;
                if (rollingBalance > sunsetLimit)
                {
                    sunsetFunds = rollingBalance - sunsetLimit;
                    rollingBalance = sunsetLimit;
                }

                var shortfall = rollingBalance < 0 ? rollingBalance * -1 : 0;
                var employerContribution = Math.Floor(shortfall*_configurationProvider.CopaymentPercentage);
                var governmentContribution = shortfall - employerContribution;

                months[i] = new MonthlyCashflow
                {
                    Date = monthDate,
                    LevyIn = Math.Round(fundsReceived, 2),
                    TrainingOut = Math.Round(trainingCostForMonth, 2),
                    Balance = rollingBalance < 0 ? 0 : Math.Round(rollingBalance, 2),
                    SunsetFunds = Math.Round(sunsetFunds, 2),
                    CoPaymentEmployer = employerContribution,
                    CoPaymentGovernment = governmentContribution,
                    FinalPaymentMade = finalPaymentMade
                };

                // Have we just balanced the account?
                if (rollingBalance < 0)
                {
                    rollingBalance = 0;
                }
            }
            return months;
        }

        private async Task<BreakdownStandard[]> ExpandCohortModels(CohortModel[] cohorts)
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
                MonthlyTrainingFraction = Math.Floor((totalCost - (totalCost * finalPaymentPercentage)) / standard.Duration);
                FinalPaymentAmount = totalCost - (MonthlyTrainingFraction * standard.Duration);
            }


            public Standard Standard { get; }
            public int Qty { get; }
            public DateTime StartDate { get; }
            public decimal MonthlyTrainingFraction { get; }
            public decimal FinalPaymentAmount { get; set; }
        }
    }
}