using System;

namespace SFA.DAS.ForecastingTool.Core.Models.FinancialForecasting
{
    public class MonthlyCashflow
    {
        public DateTime Date { get; set; }
        public decimal LevyIn { get; set; }
        public decimal TrainingOut { get; set; }
        public decimal Balance { get; set; }
        public decimal SunsetFunds { get; set; }
        public decimal CoPaymentEmployer { get; set; }
        public decimal CoPaymentGovernment { get; set; }
        public bool FinalPaymentMade { get; set; }
    }
}