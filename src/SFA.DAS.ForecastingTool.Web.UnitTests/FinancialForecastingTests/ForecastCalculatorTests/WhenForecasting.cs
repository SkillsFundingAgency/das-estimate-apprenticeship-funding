using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.FinancialForecastingTests.ForecastCalculatorTests
{
    public class WhenForecasting
    {
        private const int Paybill = 4250000;
        private const int CoPayPaybill = 4000000;
        private const int StandardCode = 1;
        private const int StandardQty = 1;

        private Mock<IStandardsRepository> _standardsRepository;
        private ForecastCalculator _calculator;
        private Mock<IConfigurationProvider> _configurationProvider;

        [SetUp]
        public void Arrange()
        {
            _standardsRepository = new Mock<IStandardsRepository>();
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode)).Returns(Task.FromResult(new Standard
            {
                Price = 6000
            }));

            _configurationProvider = new Mock<IConfigurationProvider>();
            _configurationProvider.Setup(cp => cp.LevyPercentage).Returns(0.005m);
            _configurationProvider.Setup(cp => cp.LevyAllowance).Returns(15000);
            _configurationProvider.Setup(cp => cp.LevyTopupPercentage).Returns(1.1m);
            _configurationProvider.Setup(cp => cp.CopaymentPercentage).Returns(0.1m);

            _calculator = new ForecastCalculator(_standardsRepository.Object, _configurationProvider.Object);
        }

        [Test]
        public async Task ThenItShouldReturn12MonthsOfForecast()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(12, actual.Length);
        }

        [Test]
        public async Task ThenItShouldReturnConsecutiveMonthlyDatesStartingOnApril2017()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            var startDate = new DateTime(2017, 4, 1);
            for (var i = 0; i < 12; i++)
            {
                var expectedDate = startDate.AddMonths(i);
                var actualDate = actual[i].Date;

                Assert.AreEqual(expectedDate, actualDate,
                    $"Expected actual[{i}].Date to be {expectedDate} but was {actualDate}");
            }
        }

        [Test]
        public async Task ThenItShouldReturnEveryMonthsLevyInAsOneTwelthOfPaybillAfterPercentageAndAllowanceAndTopup()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualLevyIn = actual[i].LevyIn;

                Assert.AreEqual(572.92m, actualLevyIn,
                    $"Expected actual[{i}].LevyIn to be 458.33 but was {actualLevyIn}");
            }
        }

        [Test]
        public async Task ThenItShouldReturnEveryMonthsTrainingOutAsOneTwelthOfTheTotalCohortTrainingCost()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;

                Assert.AreEqual(500, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be 400 but was {actualTrainingOut}");
            }
        }

        [Test]
        public async Task ThenItShouldKeepARollingBalance()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            Assert.AreEqual(72.92m, actual[0].Balance);
            Assert.AreEqual(145.83m, actual[1].Balance);
            Assert.AreEqual(218.75m, actual[2].Balance);
            Assert.AreEqual(291.67m, actual[3].Balance);
            Assert.AreEqual(364.58m, actual[4].Balance);
            Assert.AreEqual(437.50m, actual[5].Balance);
            Assert.AreEqual(510.42m, actual[6].Balance);
            Assert.AreEqual(583.33m, actual[7].Balance);
            Assert.AreEqual(656.25m, actual[8].Balance);
            Assert.AreEqual(729.17m, actual[9].Balance);
            Assert.AreEqual(802.08m, actual[10].Balance);
            Assert.AreEqual(875.00m, actual[11].Balance);
        }

        [Test]
        public async Task ThenItShouldHaveCopaymentOfTheDeficitAtCopaymentRate()
        {
            // Act
            var actual = await _calculator.ForecastAsync(CoPayPaybill, StandardCode, StandardQty);

            // Assert
            Assert.AreEqual(4.17m, actual[0].CoPayment);
            Assert.AreEqual(4.17m, actual[1].CoPayment);
            Assert.AreEqual(4.17m, actual[2].CoPayment);
            Assert.AreEqual(4.17m, actual[3].CoPayment);
            Assert.AreEqual(4.17m, actual[4].CoPayment);
            Assert.AreEqual(4.17m, actual[5].CoPayment);
            Assert.AreEqual(4.17m, actual[6].CoPayment);
            Assert.AreEqual(4.17m, actual[7].CoPayment);
            Assert.AreEqual(4.17m, actual[8].CoPayment);
            Assert.AreEqual(4.17m, actual[9].CoPayment);
            Assert.AreEqual(4.17m, actual[10].CoPayment);
            Assert.AreEqual(4.17m, actual[11].CoPayment);
        }
    }
}
