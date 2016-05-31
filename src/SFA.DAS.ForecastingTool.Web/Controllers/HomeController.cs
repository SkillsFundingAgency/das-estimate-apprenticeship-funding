using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStandardsRepository _standardsRepository;
        private readonly IForecastCalculator _forecastCalculator;

        public HomeController(IStandardsRepository standardsRepository, IForecastCalculator forecastCalculator)
        {
            _standardsRepository = standardsRepository;
            _forecastCalculator = forecastCalculator;
        }

        public ActionResult Welcome()
        {
            return View(new ForecastQuestionsModel());
        }

        public ActionResult Paybill(ForecastQuestionsModel model)
        {
            return View(model);
        }

        public async Task<ActionResult> TrainingCourse(TrainingCourseViewModel model)
        {
            var standards = await _standardsRepository.GetAllAsync();

            model.Standards = standards.OrderBy(s => s.Name).ToArray();

            return View(model);
        }

        public async Task<ActionResult> Results(ResultsViewModel model)
        {
            model.Results = await _forecastCalculator.ForecastAsync(model.Paybill, model.SelectedStandard.Code, model.SelectedStandard.Qty);
            return View(model);
        }
    }
}