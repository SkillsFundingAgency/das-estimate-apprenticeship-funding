using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ResultsViewModel : ForecastQuestionsModel
    {
        public MonthlyCashflow[] Results { get; set; }
        public decimal LevyAmount { get; set; }
        public decimal LevyFundingReceived { get; set; }
        public string TopPercentageForDisplay { get; set; }
        public bool CanAddPeriod { get; set; }
        public string NextPeriodUrl { get; set; }
    }
}