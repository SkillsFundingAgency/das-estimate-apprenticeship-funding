using System;
using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Core.Models.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public class ForecastCalculator : IForecastCalculator
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly ICalculatorSettings _calculatorSettings;


        public ForecastCalculator(IApprenticeshipRepository apprenticeshipRepository, ICalculatorSettings calculatorSettings)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _calculatorSettings = calculatorSettings;
        }


        public async Task<ForecastResult> ForecastAsync(long paybill, int englishFraction)
        {
            var monthlyLevyPaid = Math.Floor(((paybill * _calculatorSettings.LevyPercentage) - _calculatorSettings.LevyAllowance) / 12);
            if (monthlyLevyPaid < 0) // Non-levy payer
            {
                monthlyLevyPaid = 0;
            }
            if (monthlyLevyPaid == 0 && paybill >= 3000001)
            {
                monthlyLevyPaid = 1;
            }

            var levyPaid = monthlyLevyPaid * 12;

            var decimalEnglishFraction = englishFraction / 100m;
            var fundingReceived = Math.Ceiling((monthlyLevyPaid * decimalEnglishFraction) * _calculatorSettings.LevyTopupPercentage) * 12;

            return new ForecastResult
            {
                MonthlyLevyPaid = monthlyLevyPaid,
                FundingReceived = fundingReceived,
                LevyPaid = levyPaid,
                UserFriendlyTopupPercentage = (int)Math.Round((_calculatorSettings.LevyTopupPercentage - 1) * 100, 0)
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
                UserFriendlyTopupPercentage = (int)Math.Round((_calculatorSettings.LevyTopupPercentage - 1) * 100, 0),
                Breakdown = breakdown
            };
        }



        private async Task<MonthlyCashflow[]> CalculateBreakdown(CohortModel[] cohorts, decimal fundingReceived, int duration)
        {
            var standards = await ExpandCohortModels(cohorts);
            var startDate = new DateTime(2017, 5, 1);

            var monthlyFunding = fundingReceived / 12m;
            var sunsetLimit = monthlyFunding * _calculatorSettings.SunsettingPeriod;

            var rollingBalance = 0m;
            var months = new MonthlyCashflow[duration];
            for (var i = 0; i < duration; i++)
            {
                var trainingCostForMonth = 0m;
                var monthDate = startDate.AddMonths(i);
                var sunsetFunds = 0m;
                var fundsReceived = monthlyFunding;
                var finalPaymentMade = false;

                foreach (var standard in standards)
                {
                    var trainingStartDate = new DateTime(standard.StartDate.Year, standard.StartDate.Month, 1);
                    var trainingEndDate = trainingStartDate.AddMonths(standard.Apprenticeship.Duration - 1);
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
                var employerContribution = Math.Floor(shortfall*_calculatorSettings.CopaymentPercentage);
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
                standards[i] = new BreakdownStandard(await _apprenticeshipRepository.GetByCodeAsync(cohorts[i].Code.ToString()),
                    cohorts[i].Qty, cohorts[i].StartDate, _calculatorSettings.FinalTrainingPaymentPercentage);
            }
            return standards;
        }



        private class BreakdownStandard
        {
            public BreakdownStandard(Apprenticeship apprenticeship, int qty, DateTime startDate, decimal finalPaymentPercentage)
            {
                Apprenticeship = apprenticeship;
                Qty = qty;
                StartDate = startDate;

                var totalCost = apprenticeship.Price * qty;
                MonthlyTrainingFraction = Math.Floor((totalCost - (totalCost * finalPaymentPercentage)) / apprenticeship.Duration);
                FinalPaymentAmount = totalCost - (MonthlyTrainingFraction * apprenticeship.Duration);
            }


            public Apprenticeship Apprenticeship { get; }
            public int Qty { get; }
            public DateTime StartDate { get; }
            public decimal MonthlyTrainingFraction { get; }
            public decimal FinalPaymentAmount { get; set; }
        }
    }
}