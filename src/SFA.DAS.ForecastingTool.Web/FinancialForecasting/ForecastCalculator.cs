using System;
using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
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

        public async Task<ForecastResult> ForecastAsync(int paybill, int englishFraction, int standardCode, int standardQty, DateTime standardStartDate, int duration)
        {
            var levyPaid = (paybill * _configurationProvider.LevyPercentage) - _configurationProvider.LevyAllowance;
            if (levyPaid < 0) // Non-levy payer
            {
                levyPaid = 0;
            }

            var decimalEnglishFraction = englishFraction / 100m;
            var fundingReceived = (levyPaid * decimalEnglishFraction) * _configurationProvider.LevyTopupPercentage;

            var breakdown = await CalculateBreakdown(standardCode, standardQty, standardStartDate, fundingReceived, duration);

            return new ForecastResult
            {
                FundingReceived = fundingReceived,
                LevyPaid = levyPaid,
                UserFriendlyTopupPercentage = (int)Math.Round((_configurationProvider.LevyTopupPercentage - 1) * 100, 0),
                Breakdown = breakdown
            };
        }

        private async Task<MonthlyCashflow[]> CalculateBreakdown(int standardCode, int standardQty, DateTime standardStartDate, decimal fundingReceived, int duration)
        {
            var standard = await _standardsRepository.GetByCodeAsync(standardCode);
            var totalTrainingCost = standard?.Price * standardQty ?? 0;
            var monthlyTrainingFraction = totalTrainingCost / (decimal)(standard?.Duration ?? 1);

            var startDate = new DateTime(2017, 4, 1);
            var trainingStartDate = new DateTime(standardStartDate.Year, standardStartDate.Month, 1);
            var trainingEndDate = trainingStartDate.AddMonths(standard?.Duration ?? 1);

            var monthlyFunding = fundingReceived / 12m;

            var rollingBalance = 0m;
            var months = new MonthlyCashflow[duration];
            for (var i = 0; i < duration; i++)
            {
                var monthDate = startDate.AddMonths(i);
                var trainingHasStarted = monthDate.CompareTo(trainingStartDate) >= 0;
                var trainingHasFinished = monthDate.CompareTo(trainingEndDate) > 0;
                var trainingCostForMonth = trainingHasStarted && !trainingHasFinished ? monthlyTrainingFraction : 0;

                rollingBalance += monthlyFunding - trainingCostForMonth;

                months[i] = new MonthlyCashflow
                {
                    Date = monthDate,
                    LevyIn = Math.Round(monthlyFunding, 2),
                    TrainingOut = trainingHasStarted ? Math.Round(trainingCostForMonth, 2) : 0,
                    Balance = rollingBalance < 0 ? 0 : Math.Round(rollingBalance, 2),
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

    }
}