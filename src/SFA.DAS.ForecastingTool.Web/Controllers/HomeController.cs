using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using NLog;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Extensions;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IForecastCalculator _forecastCalculator;
        private readonly ICalculatorSettings _calculatorSettings;

        private readonly TelemetryClient _tc;

        public HomeController(
            IApprenticeshipRepository apprenticeshipRepository,
            IForecastCalculator forecastCalculator,
            ICalculatorSettings calculatorSettings)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _forecastCalculator = forecastCalculator;
            _calculatorSettings = calculatorSettings;

            _tc = new TelemetryClient();
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
            _tc.TrackPageView("Welcome");

            return View(new ForecastQuestionsModel());
        }

        public ActionResult Paybill(ForecastQuestionsModel model)
        {
            _tc.TrackPageView("Pay Bill");

            return View(model);
        }

        public ActionResult EnglishFraction(ForecastQuestionsModel model)
        {
            _tc.TrackPageView("English Fraction");

            return View(model);
        }

        public async Task<ActionResult> TrainingCourse(TrainingCourseViewModel model)
        {
            _tc.TrackPageView("Training Course");

            var standards = await _apprenticeshipRepository.GetAllAsync();
            model.Apprenticeships = standards.OrderBy(s => s.Name).Select(s => new ApprenticeshipModel(s)).ToArray();

            var forecastResult = await _forecastCalculator.ForecastAsync(model.Paybill, model.EnglishFraction);
            model.LevyFundingReceived = forecastResult.FundingReceived;
            model.TopPercentageForDisplay = forecastResult.UserFriendlyTopupPercentage.ToString("0");

            return View(model);
        }

        public async Task<ActionResult> Results(ResultsViewModel model)
        {
            _tc.TrackPageView("Results");

            var forecastResult = await _forecastCalculator.DetailedForecastAsync(model.Paybill, model.EnglishFraction,
                model.SelectedCohorts.ToArray(), model.Duration);

            model.LevyAmount = forecastResult.LevyPaid;
            model.MonthlyLevyPaid = forecastResult.MonthlyLevyPaid;
            model.LevyFundingReceived = forecastResult.FundingReceived;
            model.TopPercentageForDisplay = forecastResult.UserFriendlyTopupPercentage.ToString("0");
            model.Results = forecastResult.Breakdown;
            model.CanAddPeriod = model.Duration < _calculatorSettings.ForecastDuration;
            model.NextPeriodUrl = Request?.Url?.GetUrlToSegment(4) + (model.Duration + 12);

            model.SunsetPeriod = _calculatorSettings.SunsettingPeriod;

            var years = model.Duration / 12;
            model.TrainingCostForDuration = model.Results.Sum(x => x.TrainingOut);
            model.LevyFundingReceivedForDuration = model.LevyFundingReceived * years;
            model.FundingShortfallForDuration = model.Results.Sum(x => x.CoPaymentEmployer + x.CoPaymentGovernment);
            model.Allowance = _calculatorSettings.LevyAllowance;
            model.LevyPercentage = _calculatorSettings.LevyPercentage;
            return View(model);
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}