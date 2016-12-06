namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Settings
{
    public interface ICalculatorSettings
    {
        decimal LevyPercentage { get; }
        int LevyAllowance { get; }
        decimal LevyTopupPercentage { get; }
        decimal CopaymentPercentage { get; }
        decimal FinalTrainingPaymentPercentage { get; }
        int SunsettingPeriod { get; }
        int ForecastDuration { get; }
    }
}