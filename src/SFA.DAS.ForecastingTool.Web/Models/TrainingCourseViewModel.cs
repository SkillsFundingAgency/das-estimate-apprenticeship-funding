using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class TrainingCourseViewModel : ForecastQuestionsModel
    {
        public Standard[] Standards { get; set; }
    }
}