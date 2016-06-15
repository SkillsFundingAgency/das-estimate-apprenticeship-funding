﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.FinancialForecastingTests.ForecastCalculatorTests
{
    public class WhenForecasting
    {
        private const int Paybill = 4250000;
        private const int CoPayPaybill = 4000000;
        private const int NonLevyPaybill = 1000000;
        private const int EnglishFraction = 100;
        private int[] StandardCode = { 1, 2 };
        private int[] StandardQty = { 1, 1 };
        private readonly DateTime[] StandardStartDate = { new DateTime(2017, 4, 1), new DateTime(2017, 4, 1) };
        public const int Duration = 12;

        private Mock<IStandardsRepository> _standardsRepository;
        private ForecastCalculator _calculator;
        private Mock<IConfigurationProvider> _configurationProvider;
        private List<StandardModel> _myStandards;
        private List<StandardModel> _myMultipleStandards;

        [SetUp]
        public void Arrange()
        {
            //Arrange
            _myStandards = new List<StandardModel>();
            for (var i = 0; i < StandardQty.Length - 1; i++)
            {
                _myStandards.Add(new StandardModel
                {
                    Code = StandardCode[i],
                    Qty = StandardQty[i],
                    StartDate = StandardStartDate[i]
                });
            }

            _myMultipleStandards = new List<StandardModel>();
            for (var i = 0; i < StandardQty.Length; i++)
            {
                _myMultipleStandards.Add(new StandardModel
                {
                    Code = StandardCode[i],
                    Qty = StandardQty[i],
                    StartDate = StandardStartDate[i]
                });
            }


            _standardsRepository = new Mock<IStandardsRepository>();
            _standardsRepository.Setup(r => r.GetByCodeAsync(1)).Returns(Task.FromResult(new Standard
            {
                Price = 6000,
                Duration = 12
            }));
            _standardsRepository.Setup(r => r.GetByCodeAsync(2)).Returns(Task.FromResult(new Standard
            {
                Price = 1400,
                Duration = 12
            }));

            _configurationProvider = new Mock<IConfigurationProvider>();
            _configurationProvider.Setup(cp => cp.LevyPercentage).Returns(0.005m);
            _configurationProvider.Setup(cp => cp.LevyAllowance).Returns(15000);
            _configurationProvider.Setup(cp => cp.LevyTopupPercentage).Returns(1.1m);
            _configurationProvider.Setup(cp => cp.CopaymentPercentage).Returns(0.1m);
            _configurationProvider.Setup(cp => cp.FinalTrainingPaymentPercentage).Returns(0m);

            _calculator = new ForecastCalculator(_standardsRepository.Object, _configurationProvider.Object);
        }

        [Test]
        public async Task ThenItShouldReturnInstanceOfForecastResult()
        {
            // Act
            var actual = await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration);

            // Assert
            Assert.IsNotNull(actual);
        }

        [TestCase(4000000, 5000)]
        [TestCase(4500000, 7500)]
        [TestCase(1000000, 0)]
        public async Task ThenItShouldReturnCorrectLevyPaidAmount(int paybill, int expectedLevyPaid)
        {
            // Act
            var actual = await _calculator.ForecastAsync(paybill, EnglishFraction, _myStandards.ToArray(), Duration);

            // Assert
            Assert.AreEqual(expectedLevyPaid, actual.LevyPaid);
        }

        [TestCase(4000000, 5500)]
        [TestCase(4500000, 8250)]
        [TestCase(1000000, 0)]
        public async Task ThenItShouldReturnCorrectFundingReceivedAmount(int paybill, int expectedFundingReceived)
        {
            // Act
            var actual = await _calculator.ForecastAsync(paybill, EnglishFraction, _myStandards.ToArray(), Duration);

            // Assert
            Assert.AreEqual(expectedFundingReceived, actual.FundingReceived);
        }

        [TestCase(1.1, 10)]
        [TestCase(1.3, 30)]
        [TestCase(1.05, 5)]
        [TestCase(1.92, 92)]
        public async Task ThenItShouldReturnCorrectlyFormatedTopupPercentage(double configuredTopupPercentage, int expectedTopupPercentage)
        {
            // Arrange
            _configurationProvider.Setup(cp => cp.LevyTopupPercentage).Returns((decimal)configuredTopupPercentage);

            // Act
            var actual = await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration);

            // Assert
            Assert.AreEqual(expectedTopupPercentage, actual.UserFriendlyTopupPercentage);
        }

        [TestCase(12)]
        [TestCase(34)]
        [TestCase(36)]
        public async Task ThenItShouldReturnSpecifiedNumberOfMonthsOfForecast(int duration)
        {
            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), duration))?.Breakdown;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(duration, actual.Length);
        }

        [Test]
        public async Task ThenItShouldReturnConsecutiveMonthlyDatesStartingOnApril2017()
        {
            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

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
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualLevyIn = actual[i].LevyIn;

                Assert.AreEqual(572.92m, actualLevyIn,
                    $"Expected actual[{i}].LevyIn to be 572.92 but was {actualLevyIn}");
            }
        }

        [TestCase(12, 450)]
        [TestCase(18, 300)]
        [TestCase(24, 225)]
        public async Task ThenItShouldReturnTrainingOutAsTheCostOfTrainingLessFinalPaymentEvenlySpreadOverCourseDuration(int courseDuration, decimal expectedMonthlyCost)
        {
            // Arrange
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode[0])).Returns(Task.FromResult(new Standard
            {
                Price = 6000,
                Duration = courseDuration
            }));
            _configurationProvider.Setup(cp => cp.FinalTrainingPaymentPercentage).Returns(0.1m);

            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 11; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;

                Assert.AreEqual(expectedMonthlyCost, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be {expectedMonthlyCost} but was {actualTrainingOut}");
            }
        }

        [Test]
        public async Task ThenItShouldReturnTrainingOutAsOneMonthsTrainingPlusFinalPaymentAmountOnFinalMonthOfTraining()
        {
            // Arrange
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode[0])).Returns(Task.FromResult(new Standard
            {
                Price = 6000,
                Duration = 11
            }));
            _configurationProvider.Setup(cp => cp.FinalTrainingPaymentPercentage).Returns(0.1m);

            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            Assert.AreEqual(1090.91m, actual[11].TrainingOut);
        }

        [TestCase(12, 625)]
        [TestCase(18, 416.67)]
        [TestCase(24, 312.5)]
        public async Task ThenItShouldReturnEveryMonthsTrainingOutAsEvenProportionOfTrainingCostForDurationWithMultipleStandards(int courseDuration, decimal expectedMonthlyCost)
        {
            // Arrange
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode[0])).Returns(Task.FromResult(new Standard
            {
                Price = 6000,
                Duration = courseDuration
            }));
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode[1])).Returns(Task.FromResult(new Standard
            {
                Price = 1500,
                Duration = courseDuration
            }));

            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myMultipleStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;

                Assert.AreEqual(expectedMonthlyCost, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be {expectedMonthlyCost} but was {actualTrainingOut}");
            }
        }

        [TestCase(new[] { 1, 1 }, 625)]
        [TestCase(new[] { 1, 10 }, 1750)]
        [TestCase(new[] { 10, 10 }, 6250)]
        public async Task ThenItShouldCalculateTheMonthlyTrainginCostForDifferentQuantities(int[] cohortQauntities, decimal expectedMonthlyCost)
        {

            //Arrange
            var myStandards = new List<StandardModel>();
            for (var i = 0; i < StandardQty.Length; i++)
            {
                myStandards.Add(new StandardModel
                {
                    Code = StandardCode[i],
                    Qty = cohortQauntities[i],
                    StartDate = StandardStartDate[i]
                });
            }
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode[0])).Returns(Task.FromResult(new Standard
            {
                Price = 6000,
                Duration = 12
            }));
            _standardsRepository.Setup(r => r.GetByCodeAsync(StandardCode[1])).Returns(Task.FromResult(new Standard
            {
                Price = 1500,
                Duration = 12
            }));

            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;


                Assert.AreEqual(expectedMonthlyCost, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be {expectedMonthlyCost} but was {actualTrainingOut}");
            }

        }


        [Test]
        public async Task ThenItShouldOnlyStartTakingTrainingCostsFromTheStartDate()
        {
            //Arrange
            var myStandards = new List<StandardModel>();
            for (var i = 0; i < StandardQty.Length - 1; i++)
            {
                myStandards.Add(new StandardModel
                {
                    Code = StandardCode[i],
                    Qty = StandardQty[i],
                    StartDate = StandardStartDate[i].AddMonths(2)
                });
            }


            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            Assert.AreEqual(0, actual[0].TrainingOut, $"Expected actual[0].TrainingOut to be 500 but was {actual[0].TrainingOut}");
            Assert.AreEqual(0, actual[1].TrainingOut, $"Expected actual[1].TrainingOut to be 500 but was {actual[1].TrainingOut}");
            for (var i = 2; i < 12; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;

                Assert.AreEqual(500, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be 500 but was {actualTrainingOut}");
            }
        }

        [Test]
        public async Task ThenItShouldStopTakingTrainingCostsFromTheStartDatePlusStandardDuration()
        {
            //Arrange
            var myStandards = new List<StandardModel>();
            for (var i = 0; i < StandardQty.Length - 1; i++)
            {
                myStandards.Add(new StandardModel
                {
                    Code = StandardCode[i],
                    Qty = StandardQty[i],
                    StartDate = StandardStartDate[i].AddMonths(-3)
                });
            }

            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 10; i++)
            {
                var actualTrainingOut = actual[i].TrainingOut;

                Assert.AreEqual(500, actualTrainingOut,
                    $"Expected actual[{i}].TrainingOut to be 500 but was {actualTrainingOut}");
            }
            Assert.AreEqual(0, actual[10].TrainingOut, $"Expected actual[10].TrainingOut to be 0 but was {actual[10].TrainingOut}");
            Assert.AreEqual(0, actual[11].TrainingOut, $"Expected actual[11].TrainingOut to be 0 but was {actual[11].TrainingOut}");
        }

        [Test]
        public async Task ThenItShouldKeepARollingBalance()
        {
            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

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
            var actual = (await _calculator.ForecastAsync(CoPayPaybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

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

        [Test]
        public async Task ThenItShouldReturnEveryMonthsLevyInAs0IfPaybillApplicableForLevy()
        {
            // Act
            var actual = (await _calculator.ForecastAsync(NonLevyPaybill, EnglishFraction, _myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualLevyIn = actual[i].LevyIn;

                Assert.AreEqual(0m, actualLevyIn,
                    $"Expected actual[{i}].LevyIn to be 0 but was {actualLevyIn}");
            }
        }

        [Test]
        public async Task ThenItShouldApplyEnglishFractionToCalculateFundingReceived()
        {
            // Act
            var actual = await _calculator.ForecastAsync(CoPayPaybill, 80, _myStandards.ToArray(), Duration);

            // Assert
            Assert.AreEqual(4400, actual.FundingReceived);
        }

        [Test]
        public async Task ThenItShouldApplyEnglishFractionToCalculateMonthlyLevyIn()
        {
            // Act
            var actual = (await _calculator.ForecastAsync(CoPayPaybill, 80, _myStandards.ToArray(), Duration))?.Breakdown;

            // Assert
            for (var i = 0; i < 12; i++)
            {
                var actualLevyIn = actual[i].LevyIn;

                Assert.AreEqual(366.67m, actualLevyIn,
                    $"Expected actual[{i}].LevyIn to be 366.67 but was {actualLevyIn}");
            }
        }

        [Test]
        public async Task ThenItShouldSunsetFundsWhenTheBalanceExceeds18TimesMonthlyFunding()
        {
            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, new StandardModel[0], 24))?.Breakdown;

            // Assert
            for (var i = 17; i < 24; i++)
            {
                var actualBalance = actual[i].Balance;

                Assert.AreEqual(10312.50m, actualBalance,
                    $"Expected actual[{i}].Balance to be 10312.50 but was {actualBalance}");
            }
        }

        [Test]
        public async Task ThenItShouldIncludeHowMuchFundsWereSunsettedWhenApplicable()
        {
            // Act
            var actual = (await _calculator.ForecastAsync(Paybill, EnglishFraction, new StandardModel[0], 24))?.Breakdown;

            for (var i = 0; i < 18; i++)
            {
                var actualSunsetFunds = actual[i].SunsetFunds;

                Assert.AreEqual(0m, actualSunsetFunds,
                    $"Expected actual[{i}].SunsetFunds to be 0 but was {actualSunsetFunds}");
            }
            // Assert
            for (var i = 18; i < 24; i++)
            {
                var actualSunsetFunds = actual[i].SunsetFunds;

                Assert.AreEqual(572.92m, actualSunsetFunds,
                    $"Expected actual[{i}].SunsetFunds to be 572.92 but was {actualSunsetFunds}");
            }
        }
    }
}
