namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ForecastQuestionsModel
    {
        public string ErrorMessage { get; set; }
        public int Paybill { get; set; }
        public int EnglishFraction { get; set; }
        public StandardModel SelectedStandard { get; set; }
        public int Duration { get; set; }
    }
}