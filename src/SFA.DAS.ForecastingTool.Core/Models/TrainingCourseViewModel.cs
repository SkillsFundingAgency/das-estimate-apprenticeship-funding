namespace SFA.DAS.ForecastingTool.Core.Models
{
    public class TrainingCourseViewModel : ForecastQuestionsModel
    {
        public StandardModel[] Standards { get; set; }
        public decimal LevyFundingReceived { get; set; }
        public string TopPercentageForDisplay { get; set; }
    }
}