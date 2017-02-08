namespace SFA.DAS.ForecastingTool.Core.Models
{
    public class TrainingCourseViewModel : ForecastQuestionsModel
    {
        public ApprenticeshipModel[] Apprenticeships { get; set; }
        public decimal LevyFundingReceived { get; set; }
        public string TopPercentageForDisplay { get; set; }
    }
}