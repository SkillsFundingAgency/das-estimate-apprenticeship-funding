using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ResultsViewModel : ForecastQuestionsModel
    {
        public MonthlyCashflow[] Results { get; set; }
    }
}