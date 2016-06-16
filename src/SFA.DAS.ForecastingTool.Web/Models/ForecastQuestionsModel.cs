using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ForecastQuestionsModel
    {
        public ForecastQuestionsModel()
        {
            SelectedCohorts = new List<CohortModel>();
        }
        public string ErrorMessage { get; set; }
        public long Paybill { get; set; }
        public int EnglishFraction { get; set; }
        public List<CohortModel> SelectedCohorts { get; set; }
        public int Duration { get; set; }

        public string GetCohortsUrl()
        {
            if (!SelectedCohorts.Any())
            {
                return string.Empty;
            }
            return SelectedCohorts.Select(x => $"{x.Qty}x{x.Code}-{x.StartDate.ToString("yyyy-MM-dd")}")
                .Aggregate((x, y) => $"{x}_{y}");
        }
    }
}