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
        private const string ErrorMessageRouteValueKey = "ErrorMessage";
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
            var actual = _parser.Parse($"{BasePath}/987654321");

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

        [TestCase(BasePath + "/987654321/4x34", 4, 34)]
        [TestCase(BasePath + "/987654321/12x34", 12, 34)]
        public void ThenItShouldIncludeTheStandardInfoInRouteValuesIfInPath(string path, int expectedQty, int expectedCode)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(StandardQtyRouteValueKey));
            Assert.AreEqual(expectedQty, actual.RouteValues[StandardQtyRouteValueKey]);

            Assert.IsTrue(actual.RouteValues.ContainsKey(StandardCodeRouteValueKey));
            Assert.AreEqual(expectedCode, actual.RouteValues[StandardCodeRouteValueKey]);

            Assert.IsTrue(actual.RouteValues.ContainsKey(StandardNameRouteValueKey));
            Assert.AreEqual("Unit tester", actual.RouteValues[StandardNameRouteValueKey]);
        }
        
        [TestCase(BasePath + "/abc")]
        [TestCase(BasePath + "/abc/4x34")]
        [TestCase(BasePath + "/2147483648")]
        [TestCase(BasePath + "/2147483648/4x34")]
        public void ThenItShouldReturnPaybillIfValueInPathIsNotValidInt(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual("Paybill", actual.ActionName);
        }

        [TestCase(BasePath + "/abc")]
        [TestCase(BasePath + "/abc/4x34")]
        [TestCase(BasePath + "/2147483648")]
        [TestCase(BasePath + "/2147483648/4x34")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfValueInPathIsNotValidInt(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Paybill is not a valid entry", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/2999999")]
        [TestCase(BasePath + "/2999999/4x34")]
        public void ThenItShouldReturnPaybillIfValueInPathIsBelowLevyLimit(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual("Paybill", actual.ActionName);
        }

        [TestCase(BasePath + "/2999999")]
        [TestCase(BasePath + "/2999999/4x34")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfValueInPathIsBelowLevyLimit(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("You paybill indicates you will not be a levy payer. You will not pay levy until your paybill is £3,000,000 or more", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/2999999")]
        [TestCase(BasePath + "/2999999/4x34")]
        public void ThenItShouldIncludeThePaybillInRouteValuesIfValueInPathIsBelowLevyLimit(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(PaybillRouteValueKey));
            Assert.AreEqual(2999999, actual.RouteValues[PaybillRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/ax34")]
        [TestCase(BasePath + "/987654321/2xa")]
        [TestCase(BasePath + "/987654321/axa")]
        [TestCase(BasePath + "/987654321/abc")]
        [TestCase(BasePath + "/987654321/4x99")]
        public void ThenItShouldReturnTrainingCourseIfStandardIsNotValidOrStandardNotFound(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual("TrainingCourse", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/ax34")]
        [TestCase(BasePath + "/987654321/2xa")]
        [TestCase(BasePath + "/987654321/axa")]
        [TestCase(BasePath + "/987654321/abc")]
        [TestCase(BasePath + "/987654321/4x99")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStandardIsNotValidOrStandardNotFound(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Number of apprentices or training standard invalid", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [Test]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStandardSelectedButCohortSizeIs0()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/0x34");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Must have at least 1 apprentice to calculate. Alternatively you can skip this step", actual.RouteValues[ErrorMessageRouteValueKey]);
        }
    }
}
