using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Routing;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.InfrastructureTests.RoutingTests.UrlParserTests
{
    public class WhenParsing
    {
        private const string BasePath = "Forecast";
        private const string PaybillRouteValueKey = "Paybill";
        private const string StandardQtyRouteValueKey = "SelectedStandard.Qty";
        private const string StandardCodeRouteValueKey = "SelectedStandard.Code";
        private const string StandardNameRouteValueKey = "SelectedStandard.Name";

        private Mock<IStandardsRepository> _standardsRepo;
        private UrlParser _parser;

        [SetUp]
        public void Arrange()
        {
            _standardsRepo = new Mock<IStandardsRepository>();
            _standardsRepo.Setup(r => r.GetByCodeAsync(34)).Returns(Task.FromResult(new Standard { Name = "Unit tester" }));

            _parser = new UrlParser(_standardsRepo.Object);
        }

        [Test]
        public void ThenItShouldReturnAnInstanceOfParsedUrl()
        {
            // Act
            var actual = _parser.Parse("");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenItShouldReturnWelcomePageIfNoPath()
        {
            // Act
            var actual = _parser.Parse("");

            // Assert
            Assert.AreEqual("Welcome", actual.ActionName);
        }

        [Test]
        public void ThenItShouldReturnPaybillIfOnlyForecastInPath()
        {
            // Act
            var actual = _parser.Parse(BasePath);

            // Assert
            Assert.AreEqual("Paybill", actual.ActionName);
        }

        [TestCase("/", "Welcome")]
        [TestCase(BasePath + "/", "Paybill")]
        [TestCase("/" + BasePath, "Paybill")]
        [TestCase("/" + BasePath + "/", "Paybill")]
        public void ThenItShouldRemoveLeadingAndOrTrailingSlashes(string path, string expectedActionName)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual(expectedActionName, actual.ActionName);
        }

        [Test]
        public void ThenItShouldReturnTrainingCourseIfPathHasOnlyAmount()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654");

            // Assert
            Assert.AreEqual("TrainingCourse", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321")]
        [TestCase(BasePath + "/987654321/4x34")]
        public void ThenItShouldIncludeThePaybillInRouteValuesIfInPath(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(PaybillRouteValueKey));
            Assert.AreEqual(987654321, actual.RouteValues[PaybillRouteValueKey]);
        }

        [Test]
        public void ThenItShouldReturnResultsIfPathHasAmountAndStandard()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/4x34");

            // Assert
            Assert.AreEqual("Results", actual.ActionName);
        }

        [Test]
        public void ThenItShouldIncludeTheStandardInfoInRouteValuesIfInPath()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/4x34");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(StandardQtyRouteValueKey));
            Assert.AreEqual(4, actual.RouteValues[StandardQtyRouteValueKey]);

            Assert.IsTrue(actual.RouteValues.ContainsKey(StandardCodeRouteValueKey));
            Assert.AreEqual(34, actual.RouteValues[StandardCodeRouteValueKey]);

            Assert.IsTrue(actual.RouteValues.ContainsKey(StandardNameRouteValueKey));
            Assert.AreEqual("Unit tester", actual.RouteValues[StandardNameRouteValueKey]);
        }
    }
}
