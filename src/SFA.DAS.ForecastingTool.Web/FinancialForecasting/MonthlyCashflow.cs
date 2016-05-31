using System;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public class MonthlyCashflow
    {
        public DateTime Date { get; set; }
        public decimal LevyIn { get; set; }
        public decimal TrainingOut { get; set; }
        public decimal Balance { get; set; }
        public decimal CoPayment { get; set; }
    }
}