namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class ForecastQuestionsModel
    {
        public int Paybill { get; set; }
        public StandardModel SelectedStandard { get; set; }
    }

    public class StandardModel
    {
        public int Code { get; set; }
        public int Qty { get; set; }
    }
}