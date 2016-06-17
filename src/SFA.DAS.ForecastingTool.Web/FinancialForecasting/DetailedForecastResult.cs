namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public class DetailedForecastResult : ForecastResult
    {
        public MonthlyCashflow[] Breakdown { get; set; }
    }
}