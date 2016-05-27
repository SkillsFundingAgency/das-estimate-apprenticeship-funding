using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Routing;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.InfrastructureTests.RoutingTests.UrlParserTests
{
    public class WhenParsing
    {
        private const string BasePath = "Forecast";
        private const string PaybillRouteValueKey = "Paybill";

        [Test]
        public void ThenItShouldReturnAnInstanceOfParsedUrl()
        {
            // Act
            var actual = UrlParser.Parse("");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenItShouldReturnWelcomePageIfNoPath()
        {
            // Act
            var actual = UrlParser.Parse("");

            // Assert
            Assert.AreEqual("Welcome", actual.ActionName);
        }

        [Test]
        public void ThenItShouldReturnPaybillIfOnlyForecastInPath()
        {
            // Act
            var actual = UrlParser.Parse(BasePath);

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
            var actual = UrlParser.Parse(path);

            // Assert
            Assert.AreEqual(expectedActionName, actual.ActionName);
        }

        [Test]
        public void ThenItShouldReturnTrainingCourseIfPathHasOnlyAmount()
        {
            // Act
            var actual = UrlParser.Parse($"{BasePath}/987654");

            // Assert
            Assert.AreEqual("TrainingCourse", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321")]
        [TestCase(BasePath + "/987654321/4x34")]
        public void ThenItShouldIncludeThePaybillInRouteValuesIfInPath(string path)
        {
            // Act
            var actual = UrlParser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(PaybillRouteValueKey));
            Assert.AreEqual(987654321, actual.RouteValues[PaybillRouteValueKey]);
        }

        [Test]
        public void ThenItShouldReturnResultsIfPathHasAmountAndStandard()
        {
            // Act
            var actual = UrlParser.Parse($"{BasePath}/987654321/4x34");

            // Assert
            Assert.AreEqual("Results", actual.ActionName);
        }
    }
}
