using System.Collections.Generic;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ForecastQuestionsModel
    {
        public ForecastQuestionsModel()
        {
            SelectedStandards = new List<StandardModel>();
        }
        public string ErrorMessage { get; set; }
        public long Paybill { get; set; }
        public int EnglishFraction { get; set; }
        public List<StandardModel> SelectedStandards { get; set; }
        public int Duration { get; set; }
    }
}