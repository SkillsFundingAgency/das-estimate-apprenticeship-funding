using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Core.Models.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Models;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public interface IForecastCalculator
    {
        Task<ForecastResult> ForecastAsync(long paybill, int englishFraction);
        Task<DetailedForecastResult> DetailedForecastAsync(long paybill, int englishFraction, CohortModel[] cohorts, int duration);
    }
}