using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Logger.Error(filterContext.Exception, filterContext.Exception.Message);
            }
            base.OnException(filterContext);
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
            model.Standards = standards.OrderBy(s => s.Name).Select(s => new StandardModel(s)).ToArray();

            var forecastResult = await _forecastCalculator.ForecastAsync(model.Paybill, model.EnglishFraction);
            model.LevyFundingReceived = forecastResult.FundingReceived;
            model.TopPercentageForDisplay = forecastResult.UserFriendlyTopupPercentage.ToString("0");

            return View(model);
        }

        public async Task<ActionResult> Results(ResultsViewModel model)
        {
            var forecastResult = await _forecastCalculator.DetailedForecastAsync(model.Paybill, model.EnglishFraction,
                model.SelectedCohorts.ToArray(), model.Duration);

            model.LevyAmount = forecastResult.LevyPaid;
            model.MonthlyLevyPaid = forecastResult.MonthlyLevyPaid;
            model.LevyFundingReceived = forecastResult.FundingReceived;
            model.TopPercentageForDisplay = forecastResult.UserFriendlyTopupPercentage.ToString("0");
            model.Results = forecastResult.Breakdown;
            model.CanAddPeriod = model.Duration < 36;
            model.NextPeriodUrl = Request?.Url?.GetUrlToSegment(4) + (model.Duration + 12);

            var years = model.Duration / 12;
            model.TrainingCostForDuration = model.Results.Sum(x => x.TrainingOut);
            model.LevyFundingReceivedForDuration = model.LevyFundingReceived * years;
            model.FundingShortfallForDuration = model.Results.Sum(x => x.CoPaymentEmployer + x.CoPaymentGovernment);
            model.Allowance = _configurationProvider.LevyAllowance;
            model.LevyPercentage = _configurationProvider.LevyPercentage;
            return View(model);
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}