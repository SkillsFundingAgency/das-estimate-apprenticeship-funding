using System.Configuration;
using SFA.DAS.ForecastingTool.Web.Extensions;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Settings
{
    public class CalculatorSettings : ICalculatorSettings
    {
        public decimal LevyPercentage => GetSetting<decimal>("Levy:Percentage") ?? 1;

        public int LevyAllowance => GetSetting<int>("Levy:Allowance") ?? 0;

        public decimal LevyTopupPercentage => GetSetting<decimal>("Levy:TopupPercentage") ?? 1;

        public decimal CopaymentPercentage => GetSetting<decimal>("Copay:Percentage") ?? 1;

        public decimal FinalTrainingPaymentPercentage => GetSetting<decimal>("Training:FinalPaymentPercentage") ?? 1;
        
        public int SunsettingPeriod => GetSetting<int>("Levy:SunsettingPeriod") ?? 18;

        public int ForecastDuration => GetSetting<int>("Levy:ForecastDuration") ?? 36;

        private T? GetSetting<T>(string key) where T : struct
        {
            return ConfigurationManager.AppSettings[key].Convert<T>();
        }
    }
}