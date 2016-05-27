using System;
using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public interface IForecastCalculator
    {
        Task<MonthlyCashflow[]> ForecastAsync(int paybill, int standardCode, int standardQty);
    }

    public class ForecastCalculator : IForecastCalculator
    {
        private const decimal LevyPercentage = 0.005m;
        private const int LevyAllowance = 15000;
        private const decimal LevyTopupPercentage = 1.1m;

        private readonly IStandardsRepository _standardsRepository;

        public ForecastCalculator(IStandardsRepository standardsRepository)
        {
            _standardsRepository = standardsRepository;
        }

        public async Task<MonthlyCashflow[]> ForecastAsync(int paybill, int standardCode, int standardQty)
        {
            var standard = await _standardsRepository.GetByCodeAsync(standardCode);
            var duration = 12;

            var totalTrainingCost = standard.Price * standardQty;
            var monthlyTrainingFraction = totalTrainingCost / 15m;

            var startDate = new DateTime(2017, 4, 1);

            var annualLevy = (paybill * LevyPercentage) - LevyAllowance;
            var monthlyLevy = annualLevy / 12m;

            var rollingBalance = 0m;
            var months = new MonthlyCashflow[duration];
            for (var i = 0; i < duration; i++)
            {
                var levyIn = monthlyLevy * LevyTopupPercentage;
                var trainingOut = i == duration - 1 ? monthlyTrainingFraction * 4 : monthlyTrainingFraction;

                rollingBalance += levyIn - trainingOut;

                months[i] = new MonthlyCashflow
                {
                    Date = startDate.AddMonths(i),
                    LevyIn = Math.Round(levyIn, 2),
                    TrainingOut = Math.Round(trainingOut, 2),
                    Balance = rollingBalance < 0 ? 0 : Math.Round(rollingBalance, 2),
                    CoPayment = rollingBalance < 0 ? Math.Round(rollingBalance * -1, 2) : 0
                };
            }
            return months;
        }
    }

    public class MonthlyCashflow
    {
        public DateTime Date { get; set; }
        public decimal LevyIn { get; set; }
        public decimal TrainingOut { get; set; }
        public decimal Balance { get; set; }
        public decimal CoPayment { get; set; }
    }
}