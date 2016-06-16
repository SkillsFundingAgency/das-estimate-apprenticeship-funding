using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class TrainingCourseViewModel : ForecastQuestionsModel
    {
        public StandardModel[] Standards { get; set; }
    }
}