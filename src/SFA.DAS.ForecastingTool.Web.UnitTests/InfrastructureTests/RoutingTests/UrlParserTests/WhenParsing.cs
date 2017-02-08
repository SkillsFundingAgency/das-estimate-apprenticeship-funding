using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Routing;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.InfrastructureTests.RoutingTests.UrlParserTests
{
    public class WhenParsing
    {
        private const string BasePath = "Forecast";
        private const string ErrorMessageRouteValueKey = "ErrorMessage";
        private const string PaybillRouteValueKey = "Paybill";
        private const string EnglishFractionRouteValueKey = "EnglishFraction";
        private const string StandardQtyRouteValueKey = "SelectedCohorts[{0}].Qty";
        private const string StandardCodeRouteValueKey = "SelectedCohorts[{0}].Code";
        private const string StandardNameRouteValueKey = "SelectedCohorts[{0}].Name";
        private const string StandardStartDateRouteValueKey = "SelectedCohorts[{0}].StartDate";
        private const string DurationRouteValueKey = "Duration";

        private Mock<IStandardsRepository> _standardsRepo;
        private Mock<ICalculatorSettings> _configurationProvider;
        private UrlParser _parser;

        [SetUp]
        public void Arrange()
        {
            _configurationProvider = new Mock<ICalculatorSettings>();
            _configurationProvider.Setup(x => x.ForecastDuration).Returns(36);

            _standardsRepo = new Mock<IStandardsRepository>();
            _standardsRepo.Setup(r => r.GetByCodeAsync("34")).Returns(Task.FromResult(new Apprenticeship { Name = "Unit tester" }));
            _standardsRepo.Setup(r => r.GetByCodeAsync("43621")).Returns(Task.FromResult(new Apprenticeship { Name = "Unit tester" }));
            _standardsRepo.Setup(r => r.GetByCodeAsync("12")).Returns(Task.FromResult(new Apprenticeship { Name = "Unit tester" }));

            _parser = new UrlParser(_standardsRepo.Object, _configurationProvider.Object);
        }

        [Test]
        public void ThenItShouldReturnAnInstanceOfParsedUrl()
        {
            // Act
            var actual = _parser.Parse("", "");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenItShouldReturnWelcomePageIfNoPath()
        {
            // Act
            var actual = _parser.Parse("", "");

            // Assert
            Assert.AreEqual("Welcome", actual.ActionName);
        }

        [Test]
        public void ThenItShouldReturnPaybillIfOnlyForecastInPath()
        {
            // Act
            var actual = _parser.Parse(BasePath, "");

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
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.AreEqual(expectedActionName, actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80", 987654321)]
        [TestCase(BasePath + "/987654321/80/4x34x0417", 987654321)]
        [TestCase(BasePath + "/2147484000/80", 2147484000)]
        [TestCase(BasePath + "/2147484000/80/4x34x0417", 2147484000)]
        public void ThenItShouldIncludeThePaybillInRouteValuesIfInPath(string path, long expectedPaybill)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(PaybillRouteValueKey));
            Assert.AreEqual(expectedPaybill, actual.RouteValues[PaybillRouteValueKey]);
        }

        [Test]
        public void ThenItShouldReturnResultsIfPathHasAmountAndFractionAndStandardButNotDuration()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/80/4x34x0417", "");

            // Assert
            Assert.AreEqual("TrainingCourse", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80/4x34x0517", new[] { 4 }, new[] { "34" }, new[] { "2017-05-01" })]
        [TestCase(BasePath + "/987654321/80/4x43621x0517", new[] { 4 }, new[] { "43621" }, new[] { "2017-05-01" })]
        [TestCase(BasePath + "/987654321/80/12x34x0218", new[] { 12 }, new[] { "34" }, new[] { "2018-02-1" })]
        [TestCase(BasePath + "/987654321/80/12x34x0218_10x12x0318", new[] { 12, 10 }, new[] { "34", "12" }, new[] { "2018-02-1", "2018-03-1" })]
        public void ThenItShouldIncludeTheStandardInfoInRouteValuesIfInPath(string path, int[] expectedQty, string[] expectedCode, string[] expectedStartDate)
        {
            // Act
            var actual = _parser.Parse(path, "");

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
        [TestCase(BasePath + "/abc/80/4x34x0417")]
        [TestCase(BasePath + "/512409557603043101")]
        [TestCase(BasePath + "/512409557603043101/80")]
        [TestCase(BasePath + "/512409557603043101/80/4x34x0417")]
        public void ThenItShouldReturnPaybillIfValueInPathIsNotValidLongOrExceedsMaxPaybill(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.AreEqual("Paybill", actual.ActionName);
        }

        [TestCase(BasePath + "/abc")]
        [TestCase(BasePath + "/abc/80")]
        [TestCase(BasePath + "/abc/4x34x0417")]
        [TestCase(BasePath + "/512409557603043101")]
        [TestCase(BasePath + "/512409557603043101/80")]
        [TestCase(BasePath + "/512409557603043101/4x34x0417")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfValueInPathIsNotValidLongOrExceedsMaxPaybill(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Enter the amount of your organisation’s UK payroll", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/0")]
        [TestCase(BasePath + "/0/80")]
        [TestCase(BasePath + "/0/4x34x0417")]
        [TestCase(BasePath + "/-1")]
        [TestCase(BasePath + "/-1/80")]
        [TestCase(BasePath + "/-1/4x34x0417")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfValueInPathIsNotPositive(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("Enter the amount of your organisation’s UK payroll", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/ax34x0417")]
        [TestCase(BasePath + "/987654321/80/2xax0417")]
        [TestCase(BasePath + "/987654321/80/axax0417")]
        [TestCase(BasePath + "/987654321/80/abcx0417")]
        [TestCase(BasePath + "/987654321/80/abc")]
        [TestCase(BasePath + "/987654321/80/4x99x0417")]
        public void ThenItShouldReturnTrainingCourseIfStandardIsNotValidOrStandardNotFound(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.AreEqual("TrainingCourse", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80/2xax0417")]
        [TestCase(BasePath + "/987654321/80/axax0417")]
        [TestCase(BasePath + "/987654321/80/abcx0417")]
        [TestCase(BasePath + "/987654321/80/ax34x0417")]
        [TestCase(BasePath + "/987654321/80/abc")]
        [TestCase(BasePath + "/987654321/80/4x99xabc")]
        [TestCase(BasePath + "/987654321/80/4x99x012017")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStandardIsNotValidOrStandardNotFoundOrChortSizeInvalidOrStartDateInvalid(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("You must choose an apprenticeship training course. Alternatively you can skip this step.", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/4x34x0418_2xax0417")]
        [TestCase(BasePath + "/987654321/80/4x34x0418_axax0417")]
        [TestCase(BasePath + "/987654321/80/4x34x0418_abcx0417")]
        [TestCase(BasePath + "/987654321/80/4x34x0418_ax34x0417")]
        [TestCase(BasePath + "/987654321/80/4x34x0418_abc")]
        [TestCase(BasePath + "/987654321/80/4x34x0418_4x99xabc")]
        [TestCase(BasePath + "/987654321/80/4x34x0418_4x99x012017")]
        public void ThenItShouldIncludeErrorMessageIfOneCohortEntryIsCorrectAndTheAdditionalOneIsnt(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("You need to choose at least one apprentice to add another row. Alternatively you can skip this step.", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/4x99x0417")]
        [TestCase(BasePath + "/987654321/80/4x99x0017")]
        [TestCase(BasePath + "/987654321/80/4x99x9917")]
        [TestCase(BasePath + "/987654321/80/4x99x0100")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStartDateInvalid(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            Assert.AreEqual("Start date invalid, please enter a date from May 2017", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1000001x34x0118")]
        [TestCase(BasePath + "/987654321/80/1000000000000001x34x0118")]
        [TestCase(BasePath + "/987654321/80/1000000000000000000000000000000000000000001x34x0118")]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfQtyPerCohortIsGreaterThanOneMillion(string url)
        {
            // Act
            var actual = _parser.Parse(url, "");

            //Assert
            Assert.IsTrue(actual.IsErrored);
        }

        [Test]
        public void ThenItShouldIncludeErrorMessageInRouteValuesIfStandardSelectedButCohortSizeIs0()
        {
            // Act
            var actual = _parser.Parse($"{BasePath}/987654321/80/0x34x0517", "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(ErrorMessageRouteValueKey));
            Assert.AreEqual("You must choose an apprenticeship training course. Alternatively you can skip this step.", actual.RouteValues[ErrorMessageRouteValueKey]);
        }

        [Test]
        public void ThenItShouldReturnEnglishFractionIfPathHasPaybillOnly()
        {
            // Act
            var actual = _parser.Parse(BasePath + "/12345678", "");

            // Assert
            Assert.AreEqual("EnglishFraction", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/abc")]
        [TestCase(BasePath + "/987654321/abc/1x34x0417")]
        [TestCase(BasePath + "/987654321/1.1")]
        [TestCase(BasePath + "/987654321/0")]
        [TestCase(BasePath + "/987654321/-1")]
        public void ThenItShouldReturnEnglishFractionIfPathContainsInvalidDecimalPercentage(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.AreEqual("EnglishFraction", actual.ActionName);
        }

        [TestCase(BasePath + "/987654321/80")]
        [TestCase(BasePath + "/987654321/80/1x34x0417")]
        public void ThenItShouldIncludeEnglishFractionInRouteValuesIfValidEnglishFractionInUrl(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(EnglishFractionRouteValueKey));
            Assert.AreEqual(80, actual.RouteValues[EnglishFractionRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34x0517/abc")]
        public void ThenItShouldDefaultDurationTo12WhenIncorrectInUrl(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(12, actual.RouteValues[DurationRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34x0517/13", 12)]
        [TestCase(BasePath + "/987654321/80/1x34x0517/23", 12)]
        [TestCase(BasePath + "/987654321/80/1x34x0517/25", 24)]
        public void ThenItShouldReturnDurationAsTheLowestMultipleOf12IfUrlValueIsNotMultipleItself(string path, int expectedDuration)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(expectedDuration, actual.RouteValues[DurationRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34x0517/6")]
        [TestCase(BasePath + "/987654321/80/1x34x0517/0")]
        public void ThenItShouldReturnDurationAs12IfCorrectedUrlValueLessThan12(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(12, actual.RouteValues[DurationRouteValueKey]);
        }

        [TestCase(BasePath + "/987654321/80/1x34x0517/37")]
        [TestCase(BasePath + "/987654321/80/1x34x0517/48")]
        public void ThenItShouldReturnDurationAs36IfCorrectedUrlValueMoreThan36(string path)
        {
            // Act
            var actual = _parser.Parse(path, "");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(DurationRouteValueKey));
            Assert.AreEqual(36, actual.RouteValues[DurationRouteValueKey]);
        }

        [Test]
        public void ThenItShouldAllowTrainingCourseToBeSkipped()
        {
            // Act
            var actual = _parser.Parse(BasePath + "/12345678/100/0x0/12", "");

            // Assert
            Assert.AreEqual("Results", actual.ActionName);
        }


        [Test]
        public void ThenItShouldPutPreviousPaybillInRouteValuesIfEditingPaybill()
        {
            // Act
            var actual = _parser.Parse(BasePath, "?previousAnswer=12345678");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(PaybillRouteValueKey));
            Assert.AreEqual(12345678, actual.RouteValues[PaybillRouteValueKey]);
        }

        [Test]
        public void ThenItShouldPutPreviousEnglishPercentageInRouteValuesIfEditingEnglishPercentage()
        {
            // Act
            var actual = _parser.Parse(BasePath + "/4000000/", "?previousAnswer=76");

            // Assert
            Assert.IsTrue(actual.RouteValues.ContainsKey(EnglishFractionRouteValueKey));
            Assert.AreEqual(76, actual.RouteValues[EnglishFractionRouteValueKey]);
        }

        [Test]
        public void ThenItShouldPutCohortsInRouteValuesIfEditingTrainingCourse()
        {
            // Act
            var actual = _parser.Parse(BasePath + "/4000000/100/", "?previousAnswer=1x34x0517_10x12x0318");

            // Assert
            AssertStandardRouteValuesArePresentAndCorrect(actual, 0, 1, "34", new DateTime(2017, 5, 1));
            AssertStandardRouteValuesArePresentAndCorrect(actual, 1, 10, "12", new DateTime(2018, 3, 1));
        }

        [TestCase(BasePath + "/987654321/80/1x34x0417")]
        [TestCase(BasePath + "/987654321/80/1x34x0416")]
        public void ThenItShouldReturnAsIsErroredForDatesBeforeMay2017(string url)
        {
            //Act
            var actual = _parser.Parse(url,"");

            //Assert
            Assert.IsTrue(actual.IsErrored);
        }

        [Test]
        public void ThenItShouldReturnAsErroredForDatesBeforeMay2017InPreviousAnswers()
        {
            //Assert
            var actual = _parser.Parse(BasePath + "/4000000/100/", "?previousAnswer=1x34x0417_10x12x0318");

            //Act
            Assert.IsTrue(actual.IsErrored);
        }


        private void AssertStandardRouteValuesArePresentAndCorrect(ParsedUrl actual, int index, int expectedQty, string expectedCode, DateTime expectedStateDate)
        {
            var qtyKey = string.Format(StandardQtyRouteValueKey, index);
            Assert.IsTrue(actual.RouteValues.ContainsKey(qtyKey));
            Assert.AreEqual(expectedQty, actual.RouteValues[qtyKey]);

            var codeKey = string.Format(StandardCodeRouteValueKey, index);
            Assert.IsTrue(actual.RouteValues.ContainsKey(codeKey));
            Assert.AreEqual(expectedCode, actual.RouteValues[codeKey]);

            var startDateKey = string.Format(StandardStartDateRouteValueKey, index);
            Assert.IsTrue(actual.RouteValues.ContainsKey(startDateKey));
            Assert.AreEqual(expectedStateDate, actual.RouteValues[startDateKey]);
        }
    }
}
