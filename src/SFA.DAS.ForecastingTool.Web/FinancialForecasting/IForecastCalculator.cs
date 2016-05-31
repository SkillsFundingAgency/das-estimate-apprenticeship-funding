using System.Threading.Tasks;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public interface IForecastCalculator
    {
        Task<MonthlyCashflow[]> ForecastAsync(int paybill, int standardCode, int standardQty);
    }
}