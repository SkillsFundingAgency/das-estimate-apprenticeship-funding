namespace SFA.DAS.ForecastingTool.Core.Models.FinancialForecasting
{
    public class ForecastResult
    {
        public decimal FundingReceived { get; set; }
        public decimal LevyPaid { get; set; }
        public int UserFriendlyTopupPercentage { get; set; }
        public decimal MonthlyLevyPaid { get; set; }
    }
}