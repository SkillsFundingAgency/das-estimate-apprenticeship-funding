using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Controllers;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.ControllersTests.HomeControllerTests
{
    public class WhenSelectingTrainingCourse
    {
        private Standard[] _standards;
        private Mock<IStandardsRepository> _standardsRepository;
        private HomeController _controller;
        private TrainingCourseViewModel _model;

        [SetUp]
        public void Arrange()
        {
            _standards = new[]
            {
                new Standard {Code = 1, Name = "Standard B"},
                new Standard {Code = 2, Name = "Standard A"}
            };

            _standardsRepository = new Mock<IStandardsRepository>();
            _standardsRepository.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(_standards));

            _controller = new HomeController(_standardsRepository.Object, null, null);

            _model = new TrainingCourseViewModel();
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
            Assert.IsNotNull(model.Standards);
            Assert.AreEqual(2, model.Standards.Length);
            Assert.AreEqual(_standards[1].Code, model.Standards[0].Code);
            Assert.AreEqual(_standards[0].Code, model.Standards[1].Code);
        }
    }
}
