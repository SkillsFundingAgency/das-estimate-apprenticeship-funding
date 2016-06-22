using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class TrainingCourseViewModel : ForecastQuestionsModel
    {
        public StandardModel[] Standards { get; set; }
        public decimal LevyFundingReceived { get; set; }
        public string TopPercentageForDisplay { get; set; }
    }
}