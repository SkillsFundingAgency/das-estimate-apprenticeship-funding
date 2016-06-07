using System.Collections.Generic;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ForecastQuestionsModel
    {
        public string ErrorMessage { get; set; }
        public int Paybill { get; set; }
        public int EnglishFraction { get; set; }
        public List<StandardModel> SelectedStandards { get; set; }
        public int Duration { get; set; }
    }
}