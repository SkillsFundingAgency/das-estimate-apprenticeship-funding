using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStandardsRepository _standardsRepository;

        public HomeController(IStandardsRepository standardsRepository)
        {
            _standardsRepository = standardsRepository;
        }

        public HomeController()
            : this(new CachedStandardsRepository(new StandardsRepository(new DiskFileSystem()), new InProcessCacheProvider()))
        {
            
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

        public ActionResult Results(ForecastQuestionsModel model)
        {
            return View(model);
        }
    }
}