using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Web.Models;

namespace SFA.DAS.ForecastingTool.Web.FinancialForecasting
{
    public interface IForecastCalculator
    {
        
        Task<ForecastResult> ForecastAsync(long paybill, int englishFraction, CohortModel[] cohorts, int duration);
    }
}