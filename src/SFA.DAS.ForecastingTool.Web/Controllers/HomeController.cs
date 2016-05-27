using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.Models;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Welcome()
        {
            return View(new ForecastQuestionsModel());
        }

        public ActionResult Paybill(ForecastQuestionsModel model)
        {
            return View(model);
        }

        public ActionResult TrainingCourse(ForecastQuestionsModel model)
        {
            return View(model);
        }

        public ActionResult Results(ForecastQuestionsModel model)
        {
            return View(model);
        }
    }
}