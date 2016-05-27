using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.FinancialForecastingTests.ForecastCalculatorTests
{
    public class WhenForecasting
    {
        private const int Paybill = 4000000;
        private const int StandardCode = 1;
        private const int StandardQty = 1;

        private Mock<IStandardsRepository> _standardsRepository;
        private ForecastCalculator _calculator;

        [SetUp]
        public void Arrange()
        {
            _standardsRepository = new Mock<IStandardsRepository>();
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode)).Returns(Task.FromResult(new Standard
            {
                Price = 6000
            }));

            _calculator = new ForecastCalculator(_standardsRepository.Object);
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
        public async Task ThenItShouldReturnTheEveryMonthsLevyInAsOneTwelthOfPaybillAfterPercentageAndAllowanceAndTopup()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualLevyIn = actual[i].LevyIn;

                Assert.AreEqual(458.33m, actualLevyIn,
                    $"Expected actual[{i}].LevyIn to be 458.33 but was {actualLevyIn}");
            }
        }

        [Test]
        public async Task ThenItShouldReturnTheFirst11MonthsTrainingOutAsOneFifteenthOfTheTotalCohortTrainingCost()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            for (var i = 0; i < 11; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;

                Assert.AreEqual(400, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be 400 but was {actualTrainingOut}");
            }
        }

        [Test]
        public async Task ThenItShouldReturnTheLastMonthsTrainingOutAsFourFifteenthOfTheTotalCohortTrainingCost()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            Assert.AreEqual(1600, actual[11].TrainingOut);
        }

        [Test]
        public async Task ThenItShouldKeepARollingBalanceAndZeroIfNegative()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            Assert.AreEqual(58.33m, actual[0].Balance);
            Assert.AreEqual(116.67m, actual[1].Balance);
            Assert.AreEqual(175.00m, actual[2].Balance);
            Assert.AreEqual(233.33m, actual[3].Balance);
            Assert.AreEqual(291.67m, actual[4].Balance);
            Assert.AreEqual(350.00m, actual[5].Balance);
            Assert.AreEqual(408.33m, actual[6].Balance);
            Assert.AreEqual(466.67m, actual[7].Balance);
            Assert.AreEqual(525.00m, actual[8].Balance);
            Assert.AreEqual(583.33m, actual[9].Balance);
            Assert.AreEqual(641.67m, actual[10].Balance);
            Assert.AreEqual(0m, actual[11].Balance);
        }

        [Test]
        public async Task ThenItShouldHaveCopaymentOfInverseOfANegativeBalance()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, StandardCode, StandardQty);

            // Assert
            Assert.AreEqual(0m, actual[0].CoPayment);
            Assert.AreEqual(0m, actual[1].CoPayment);
            Assert.AreEqual(0m, actual[2].CoPayment);
            Assert.AreEqual(0m, actual[3].CoPayment);
            Assert.AreEqual(0m, actual[4].CoPayment);
            Assert.AreEqual(0m, actual[5].CoPayment);
            Assert.AreEqual(0m, actual[6].CoPayment);
            Assert.AreEqual(0m, actual[7].CoPayment);
            Assert.AreEqual(0m, actual[8].CoPayment);
            Assert.AreEqual(0m, actual[9].CoPayment);
            Assert.AreEqual(0m, actual[10].CoPayment);
            Assert.AreEqual(500m, actual[11].CoPayment);
        }
    }
}
