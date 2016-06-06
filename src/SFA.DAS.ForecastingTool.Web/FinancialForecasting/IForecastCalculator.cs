using System.Threading.Tasks;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public interface IForecastCalculator
    {
        Task<ForecastResult> ForecastAsync(int paybill, int englishFraction, int standardCode, int standardQty);
    }
}