using System;
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
        private const string EnglishFractionRouteValueKey = "EnglishFraction";
        private const string StandardQtyRouteValueKey = "SelectedStandards[{0}].Qty";
        private const string StandardCodeRouteValueKey = "SelectedStandards[{0}].Code";
        private const string StandardNameRouteValueKey = "SelectedStandards[{0}].Name";
        private const string StandardStartDateRouteValueKey = "SelectedStandards[{0}].StartDate";
        private const string DurationRouteValueKey = "Duration";

        private Mock<IStandardsRepository> _standardsRepo;
        private UrlParser _parser;

        [SetUp]
        public void Arrange()
        {
            _standardsRepo = new Mock<IStandardsRepository>();
            _standardsRepo.Setup(r => r.GetByCodeAsync(34)).Returns(Task.FromResult(new Standard { Name = "Unit tester" }));
            _standardsRepo.Setup(r => r.GetByCodeAsync(12)).Returns(Task.FromResult(new Standard { Name = "Unit tester" }));

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

        [TestCase(BasePath + "/987654321/80")]
        [TestCase(BasePath + "/987654321/80/4x34-2017-04-01")]
        public void ThenItShouldIncludeThePaybillInRouteValuesIfInPath(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(PaybillRouteValueKey));
            Assert.AreEqual(987654321, actual.RouteValues[PaybillRouteValueKey]);
        }

        [Test]
        public void ThenItShouldReturnResultsIfPathHasAmountAndFractionAndStandard()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/80/4x34-2017-04-01");

            // Assert
            Assert.AreEqual("Results", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80/4x34-2017-04-01", new[] { 4 }, new[] { 34 }, new[] { "2017-04-01" })]
        [TestCase(BasePath + "/987654321/80/12x34-2018-02-20", new[] { 12 }, new[] { 34 }, new[] { "2018-02-20" })]
        [TestCase(BasePath + "/987654321/80/12x34-2018-02-20_10x12-2018-03-20", new[] { 12, 10 }, new[] { 34, 12 }, new[] { "2018-02-20", "2018-03-20" })]
        public void ThenItShouldIncludeTheStandardInfoInRouteValuesIfInPath(string path, int[] expectedQty, int[] expectedCode, string[] expectedStartDate)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert

            for (var i = 0; i < expectedQty.Length; i++)
            {
                Assert.IsTrue(actual.RouteValues.ContainsKey(string.Format(StandardQtyRouteValueKey, i)));
                Assert.AreEqual(expectedQty[i], actual.RouteValues[string.Format(StandardQtyRouteValueKey, i)]);

                Assert.IsTrue(actual.RouteValues.ContainsKey(string.Format(StandardCodeRouteValueKey, i)));
                Assert.AreEqual(expectedCode[i], actual.RouteValues[string.Format(StandardCodeRouteValueKey, i)]);

                Assert.IsTrue(actual.RouteValues.ContainsKey(string.Format(StandardNameRouteValueKey, i)));
                Assert.AreEqual("Unit tester", actual.RouteValues[string.Format(StandardNameRouteValueKey, i)]);

                Assert.IsTrue(actual.RouteValues.ContainsKey(string.Format(StandardStartDateRouteValueKey, i)));
                Assert.AreEqual(DateTime.Parse(expectedStartDate[i]), actual.RouteValues[string.Format(StandardStartDateRouteValueKey, i)]);
            }
            
        }

        [TestCase(BasePath + "/abc")]
        [TestCase(BasePath + "/abc/80")]
        [TestCase(BasePath + "/abc/80/4x34-2017-04-01")]
        [TestCase(BasePath + "/2147483648")]
        [TestCase(BasePath + "/2147483648/80")]
        [TestCase(BasePath + "/2147483648/80/4x34-2017-04-01")]
        public void ThenItShouldReturnPaybillIfValueInPathIsNotValidInt(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual("Paybill", actual.ActionName);
        }

        [TestCase(BasePath + "/abc")]
        [TestCase(BasePath + "/abc/80")]
        [TestCase(BasePath + "/abc/4x34-2017-04-01")]
        [TestCase(BasePath + "/2147483648")]
        [TestCase(BasePath + "/2147483648/80")]
        [TestCase(BasePath + "/2147483648/4x34-2017-04-01")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfValueInPathIsNotValidInt(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Paybill is not a valid entry", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/0")]
        [TestCase(BasePath + "/0/80")]
        [TestCase(BasePath + "/0/4x34-2017-04-01")]
        [TestCase(BasePath + "/-1")]
        [TestCase(BasePath + "/-1/80")]
        [TestCase(BasePath + "/-1/4x34-2017-04-01")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfValueInPathIsNotPositive(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Paybill is not a valid entry", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/ax34-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/2xa-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/axa-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/abc-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/abc")]
        [TestCase(BasePath + "/987654321/80/4x99-2017-04-01")]
        public void ThenItShouldReturnTrainingCourseIfStandardIsNotValidOrStandardNotFound(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual("TrainingCourse", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80/ax34-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/2xa-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/axa-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/abc-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/abc")]
        [TestCase(BasePath + "/987654321/80/4x99-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/4x99-abc")]
        [TestCase(BasePath + "/987654321/80/4x99-99999-04-01")]
        [TestCase(BasePath + "/987654321/80/4x99-2017-56-01")]
        [TestCase(BasePath + "/987654321/80/4x99-2017-04-32")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStandardIsNotValidOrStandardNotFoundOrChortSizeInvalidOrStartDateInvalid(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Number of apprentices, training standard or start date invalid", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [Test]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStandardSelectedButCohortSizeIs0()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/80/0x34-2017-04-01");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Must have at least 1 apprentice to calculate. Alternatively you can skip this step", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [Test]
        public void ThenItShouldReturnEnglishFractionIfPathHasPaybillOnly()
        {
            // Act
            var actual = _parser.Parse(BasePath + "/12345678");

            // Assert
            Assert.AreEqual("EnglishFraction", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/abc")]
        [TestCase(BasePath + "/987654321/abc/1x34-2017-04-01")]
        [TestCase(BasePath + "/987654321/1.1")]
        [TestCase(BasePath + "/987654321/0")]
        [TestCase(BasePath + "/987654321/-1")]
        public void ThenItShouldReturnEnglishFractionIfPathContainsInvalidDecimalPercentage(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.AreEqual("EnglishFraction", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80")]
        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01")]
        public void ThenItShouldIncludeEnglishFractionInRouteValuesIfValidEnglishFractionInUrl(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(EnglishFractionRouteValueKey));
            Assert.AreEqual(80, actual.RouteValues[EnglishFractionRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01")]
        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/abc")]
        public void ThenItShouldDefaultDurationTo12WhenNotInUrlOrIncorrectInUrl(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(12, actual.RouteValues[DurationRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/13", 12)]
        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/23", 12)]
        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/25", 24)]
        public void ThenItShouldReturnDurationAsTheLowestMultipleOf12IfUrlValueIsNotMultipleItself(string path, int expectedDuration)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(expectedDuration, actual.RouteValues[DurationRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/6")]
        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/0")]
        public void ThenItShouldReturnDurationAs12IfCorrectedUrlValueLessThan12(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(12, actual.RouteValues[DurationRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/37")]
        [TestCase(BasePath + "/987654321/80/1x34-2017-04-01/48")]
        public void ThenItShouldReturnDurationAs36IfCorrectedUrlValueMoreThan36(string path)
        {
            // Act
            var actual = _parser.Parse(path);

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(36, actual.RouteValues[DurationRouteValueKey]);
        }

    }
}
