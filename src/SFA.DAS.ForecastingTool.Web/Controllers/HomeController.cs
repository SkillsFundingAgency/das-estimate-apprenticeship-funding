using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStandardsRepository _standardsRepository;
        private readonly IForecastCalculator _forecastCalculator;
        private readonly IConfigurationProvider _configurationProvider;

        public HomeController(
            IStandardsRepository standardsRepository,
            IForecastCalculator forecastCalculator,
            IConfigurationProvider configurationProvider)
        {
            _standardsRepository = standardsRepository;
            _forecastCalculator = forecastCalculator;
            _configurationProvider = configurationProvider;
        }

        public ActionResult Welcome()
        {
            return View(new ForecastQuestionsModel());
        }

        public ActionResult Paybill(ForecastQuestionsModel model)
        {
            return View(model);
        }

        public ActionResult EnglishFraction(ForecastQuestionsModel model)
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
            var forecastResult = await _forecastCalculator.ForecastAsync(model.Paybill, model.EnglishFraction,
                model.SelectedStandard.Code, model.SelectedStandard.Qty, model.SelectedStandard.StartDate, model.Duration);

            model.LevyAmount = forecastResult.LevyPaid;
            model.LevyFundingReceived = forecastResult.FundingReceived;
            model.TopPercentageForDisplay = forecastResult.UserFriendlyTopupPercentage.ToString("0");
            model.Results = forecastResult.Breakdown;
            model.CanAddPeriod = model.Duration < 36;
            model.NextPeriodUrl = Request.Url.GetUrlToSegment(4) + (model.Duration + 12);
            return View(model);
        }
    }
}