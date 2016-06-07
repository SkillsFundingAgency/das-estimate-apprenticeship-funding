using System;
using System.Collections.Generic;
using System.Linq;
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


            var standards = cohorts.Select(async s => new { Standard = await _standardsRepository.GetByCodeAsync(s.Code), Qty = s.Qty, StartDate = s.StartDate }).ToArray();

            var startDate = new DateTime(2017, 4, 1);
            

            var monthlyFunding = fundingReceived / 12m;

            var rollingBalance = 0m;
            var months = new MonthlyCashflow[duration];
            for (var i = 0; i < duration; i++)
            {
                
                var trainingCostForMonth = 0m;
                var monthDate = startDate.AddMonths(i);
                foreach (var standard in standards)
                {
                    var monthlyTrainingFraction = standard.Result.Standard.Price / (decimal)standard.Result.Standard.Duration;

                    var trainingStartDate = new DateTime(standard.Result.StartDate.Year, standard.Result.StartDate.Month, 1);
                    var trainingEndDate = trainingStartDate.AddMonths(standard.Result.Standard.Duration);
                    var trainingHasStarted = monthDate.CompareTo(trainingStartDate) >= 0;
                    var trainingHasFinished = monthDate.CompareTo(trainingEndDate) > 0;
                    trainingCostForMonth += trainingHasStarted && !trainingHasFinished ? monthlyTrainingFraction : 0;
                }


                rollingBalance += monthlyFunding - trainingCostForMonth;

                months[i] = new MonthlyCashflow
                {
                    Date = monthDate,
                    LevyIn = Math.Round(monthlyFunding, 2),
                    TrainingOut = Math.Round(trainingCostForMonth, 2),
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