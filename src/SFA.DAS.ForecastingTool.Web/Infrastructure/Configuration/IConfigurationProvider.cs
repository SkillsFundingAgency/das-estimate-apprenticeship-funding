namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration
{
    public interface IConfigurationProvider
    {
        string GetSetting(string key);
        decimal LevyPercentage { get; }
        int LevyAllowance { get; }
        decimal LevyTopupPercentage { get; }
        decimal CopaymentPercentage { get; }
        decimal FinalTrainingPaymentPercentage { get; }
        int SunsettingPeriod { get; }
    }
}