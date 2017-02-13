namespace SFA.DAS.ForecastingTool.Core.Models.FinancialForecasting
{
    public class DetailedForecastResult : ForecastResult
    {
        public MonthlyCashflow[] Breakdown { get; set; }
    }
}