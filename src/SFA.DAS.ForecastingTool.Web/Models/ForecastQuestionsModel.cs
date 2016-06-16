using System.Collections.Generic;

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
    }
}