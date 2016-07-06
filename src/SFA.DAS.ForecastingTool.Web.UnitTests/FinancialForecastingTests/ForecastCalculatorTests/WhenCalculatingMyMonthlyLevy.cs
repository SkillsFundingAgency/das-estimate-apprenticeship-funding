using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.FinancialForecastingTests.ForecastCalculatorTests
{
    public class WhenCalculatingMyMonthlyLevy
    {
        private ForecastCalculator _forecastCalculator;
        private Mock<IConfigurationProvider> _configurationProvider;

        [SetUp]
        public void Arrange()
        {
            _configurationProvider = new Mock<IConfigurationProvider>();
            _configurationProvider.Setup(x => x.LevyPercentage).Returns(0.005m);
            _configurationProvider.Setup(x => x.LevyAllowance).Returns(15000);
            _configurationProvider.Setup(x => x.LevyTopupPercentage).Returns(1.1m);

            _forecastCalculator = new ForecastCalculator(null, _configurationProvider.Object);
        }

        [TestCase(3006000, 2.0)]
        [TestCase(3003000, 1.0)]
        [TestCase(3002399, 1.0)]
        [TestCase(3000001, 1.0)]
        [TestCase(3000000, 0.0)]
        [TestCase(2999999, 0.0)]
        public async Task ThenTheMonthlyLevyPaidIsCalculatedCorrectly(long paybill, decimal expectedMonthlyLevyPaid)
        {
            //Act
            var actual = await _forecastCalculator.ForecastAsync(paybill, 100);

            //Assert
            Assert.AreEqual(expectedMonthlyLevyPaid, actual.MonthlyLevyPaid);
        }
    }
}
