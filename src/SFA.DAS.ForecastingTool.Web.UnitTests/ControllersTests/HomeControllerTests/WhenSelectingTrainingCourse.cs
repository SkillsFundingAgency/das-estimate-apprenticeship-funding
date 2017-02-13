using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SimpleInjector;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Core.Models.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Controllers;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.ControllersTests.HomeControllerTests
{
    public class WhenSelectingTrainingCourse
    {
        private Apprenticeship[] _apprenticeships;
        private Mock<IApprenticeshipRepository> _standardsRepository;
        private Mock<IForecastCalculator> _forecastCalculator;
        private HomeController _controller;
        private TrainingCourseViewModel _model;
        private Container _container;

        [SetUp]
        public void Arrange()
        {
            _container = new Infrastructure.DependencyResolution.WebRegistry().Build();

            _apprenticeships = new[]
            {
                new Apprenticeship {Code = "1", Name = "Apprenticeship B"},
                new Apprenticeship {Code = "2", Name = "Apprenticeship A"}
            };

            _standardsRepository = new Mock<IApprenticeshipRepository>();
            _standardsRepository.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(_apprenticeships));

            _forecastCalculator = new Mock<IForecastCalculator>();
            _forecastCalculator.Setup(c => c.ForecastAsync(12345678, 100))
                .Returns(Task.FromResult(new ForecastResult {FundingReceived = 987654}));

            _controller = new HomeController(_standardsRepository.Object, _forecastCalculator.Object, null, _container.GetInstance<IApprenticeshipModelMapper>());

            _model = new TrainingCourseViewModel
            {
                Paybill = 12345678,
                EnglishFraction = 100
            };
        }

        [Test]
        public async Task ThenItShouldReturnAViewResult()
        {
            // Act
            var actual = await _controller.TrainingCourse(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
        }

        [Test]
        public async Task ThenItShouldHaveATrainingCourseViewModel()
        {
            // Act
            var actual = await _controller.TrainingCourse(_model);

            // Assert
            var model = (actual as ViewResult)?.Model as TrainingCourseViewModel;
            Assert.IsNotNull(model);
        }

        [Test]
        public async Task ThenItShouldPopulateTheModelWithStandardsOrderedByName()
        {
            // Act
            var actual = await _controller.TrainingCourse(_model);

            // Assert
            var model = (actual as ViewResult)?.Model as TrainingCourseViewModel;
            Assert.IsNotNull(model.Apprenticeships);
            Assert.AreEqual(2, model.Apprenticeships.Length);
            Assert.AreEqual(_apprenticeships[1].Code, model.Apprenticeships[0].Code);
            Assert.AreEqual(_apprenticeships[0].Code, model.Apprenticeships[1].Code);
        }
    }
}
