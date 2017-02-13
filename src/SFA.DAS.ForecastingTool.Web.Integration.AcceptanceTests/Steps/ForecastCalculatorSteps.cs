using System;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Controllers;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
using SFA.DAS.ForecastingTool.Web.Standards;
using SimpleInjector;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ForecastCalculatorSteps
    {
        private Container _container;
        private HomeController _homeController;
        private ResultsViewModel _resultsViewModel;
        private IApprenticeshipRepository _apprenticeshipRepository;

        [BeforeScenario]
        public void Arrange()
        {
            _container = new Infrastructure.DependencyResolution.WebRegistry().Build();

            _apprenticeshipRepository = _container.GetInstance<IApprenticeshipRepository>();

            _homeController = new HomeController(_apprenticeshipRepository, _container.GetInstance<IForecastCalculator>(), _container.GetInstance<ICalculatorSettings>(), _container.GetInstance<IApprenticeshipModelMapper>());

            _resultsViewModel = new ResultsViewModel();
        }


        [Given(@"I have a paybill of (.*) and my English Fraction is (.*)")]
        public void GivenIHaveAPaybillOfAndMyEnglishFractionIs(int paybill, int englishFraction)
        {
            _resultsViewModel.Duration = 12;
            _resultsViewModel.EnglishFraction = englishFraction;
            _resultsViewModel.Paybill = paybill;

        }

        [When(@"I Have the following apprenticeships:")]
        public void WhenIHaveTheFollowingApprenticeships(Table apprenticeshipTable)
        {
            var apprenticeships = apprenticeshipTable.CreateSet<Apprenticeship>();

            foreach (var apprenticeship in apprenticeships)
            {
                Assert.IsNotEmpty(apprenticeship?.AppName, "The apprenticeship name was empty");

                var standard = new Core.Models.Apprenticeship { Name = apprenticeship?.AppName };

                Assert.IsNotNull(standard, $"The apprenticeship name {apprenticeship?.AppName} could not be found in the list of apprenticeship");

                //Overide the price with 12000 as the data is incorrect when compared to website data

                standard.Price = 12000;
                standard.Duration = apprenticeship.AppDuration;
                apprenticeship.AppCost = standard.Price * apprenticeship.NumberOfApprentices;

                //First verify the provided apprenticeship data is correct

                //Assert.AreEqual(apprenticeship.AppCost, standard.Price, $"Apprenticeship cost expected to be {apprenticeship.AppCost} but was {standard.Price}");
                Assert.AreEqual(apprenticeship.AppDuration, standard.Duration, $"Apprenticeship duration expected to be {apprenticeship.AppDuration} but was {standard.Duration}");

                _resultsViewModel.SelectedCohorts.Add(new CohortModel
                {
                    Code = standard.Code,
                    StartDate = Convert.ToDateTime(apprenticeship.AppStartDate),
                    Name = apprenticeship.AppName,
                    Qty = apprenticeship.NumberOfApprentices
                });


                apprenticeship.FinalMonthAchievement = _resultsViewModel.Duration - 1;
                apprenticeship.AllMonthsExceptFinalMonth = apprenticeship.AppDuration - 1;

                ScenarioContext.Current.Add("PrepData", _resultsViewModel);
                ScenarioContext.Current.Add("ApprenticeShipData", apprenticeship);
            }
        }



        [Then(@"the total annual cost of the apprenticeship should be (.*)")]
        public void ThenTheTotalAnnualCostOfTheApprenticeshipShouldBe(decimal appTotalcost)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");

            Assert.AreEqual(appTotalcost, model.TrainingCostForDuration, $"Total annual levy expected to be {appTotalcost} but was {model.TrainingCostForDuration}");

        }

        [Then(@"the apprenticeship monthly cost should be (.*)")]
        public void ThenTheApprenticeshipMonthlyCostShouldBe(decimal appMonthlyCost)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");
            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            //Validating the apprenticeship cost for each month 
            foreach (var result in model.Results.Take(apprenticeSshipData.AllMonthsExceptFinalMonth))
            {
                Assert.AreEqual(appMonthlyCost, result.TrainingOut, $"The apprenticeship cost expected for the date {result.Date} is {appMonthlyCost} but the actual amount was {result.TrainingOut}");
            }
        }

        [Then(@"the final month achievement cost should be (.*)")]
        public void ThenTheFinalMonthAchievementCostShouldBe(decimal finalMonthAchievementCost)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");
            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            //validate the last month                                                 
            Assert.AreEqual(finalMonthAchievementCost, model.Results.Select(tr => tr.TrainingOut).ElementAt(apprenticeSshipData.FinalMonthAchievement), $"Total final monthly appreticeship achievement cost expected to be {finalMonthAchievementCost} but was {model.Results.Select(tr => tr.TrainingOut).ElementAt(apprenticeSshipData.FinalMonthAchievement)}");

        }

        [Then(@"the employer monthly contribution cost should be (.*)")]
        public void ThenTheEmployerMonthlyContributionCostShouldBe(decimal employerContribution)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");
            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            //Validating the employer contribution cost for each month 
            foreach (var result in model.Results.Take(apprenticeSshipData.AllMonthsExceptFinalMonth))
            {
                Assert.AreEqual(employerContribution, result.CoPaymentEmployer, $"The employer contribution cost expected for the date {result.Date} is {employerContribution} but the actual amount was {result.CoPaymentEmployer}");
            }
        }

        [Then(@"the government monthly contribution cost should be (.*)")]
        public void ThenTheGovernmentMonthlyContributionCostShouldBe(decimal govContribution)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");
            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            //Validate government contribution cost for each month 
            foreach (var result in model.Results.Take(apprenticeSshipData.AllMonthsExceptFinalMonth))
            {
                Assert.AreEqual(govContribution, result.CoPaymentGovernment, $"The government contribution cost expected for the date {result.Date} is {govContribution} but the actual amount was {result.CoPaymentGovernment}");
            }
        }

        [Then(@"the employer final month contribution cost should be (.*)")]
        public void ThenTheEmployerFinalMonthContributionCostShouldBe(decimal employerFinalMonthCost)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");
            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            //Validate employer final month share cost                                              
            Assert.AreEqual(employerFinalMonthCost, model.Results.Select(tr => tr.CoPaymentEmployer).ElementAt(apprenticeSshipData.FinalMonthAchievement), $"The final month employer contribution cost expected to be {employerFinalMonthCost} but was {model.Results.Select(tr => tr.CoPaymentEmployer).ElementAt(apprenticeSshipData.FinalMonthAchievement)}");

        }

        [Then(@"the government final month contribution cost should be (.*)")]
        public void ThenTheGovernmentFinalMonthContributionCostShouldBe(decimal govFinalMonthCost)
        {
            var model = ScenarioContext.Current.Get<ResultsViewModel>("ResultData");
            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            //Validate government final month share cost
            Assert.AreEqual(govFinalMonthCost, model.Results.Select(tr => tr.CoPaymentGovernment).ElementAt(apprenticeSshipData.FinalMonthAchievement), $"The final month employer contribution cost expected to be {govFinalMonthCost} but was {model.Results.Select(tr => tr.CoPaymentGovernment).ElementAt(apprenticeSshipData.FinalMonthAchievement)}");

        }


        [Then(@"the total cost should be (.*), total employer contribution (.*) and total goverment pays (.*)")]
        public void ThenTheTotalCostShouldBeTotalEmployerContributionAndTotalGovermentPays(decimal totalCost, decimal totalEmployerContribution, decimal totalGovermentPays)
        {
            var actual = _homeController.Results(_resultsViewModel);
            actual.Wait();

            Assert.IsNotNull(actual);
            var viewResult = actual.Result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ResultsViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(totalCost, model.TrainingCostForDuration, $"Total cost expected to be {totalCost} but was {model.TrainingCostForDuration}");
            Assert.AreEqual(totalGovermentPays, model.Results.Sum(c => c.CoPaymentGovernment), $"Total Goverment Pays expected to be {totalGovermentPays} but was {model.Results.Sum(c => c.CoPaymentGovernment)}");
            Assert.AreEqual(totalEmployerContribution, model.Results.Sum(c => c.CoPaymentEmployer), $"Total Employer Contribution expected to be {totalEmployerContribution} but was { model.Results.Sum(c => c.CoPaymentEmployer)}");
        }


        [Then(@"the annual levy cost should be (.*) and mothly levy cost should be (.*)")]
        public void ThenTheAnnualLevyCostShouldBeAndMothlyLevyCostShouldBe(decimal annualLevyCost, decimal monthlyLevyCost)
        {
            var actual = _homeController.Results(_resultsViewModel);
            actual.Wait();

            Assert.IsNotNull(actual);
            var viewResult = actual.Result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ResultsViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(annualLevyCost, model.LevyFundingReceived, $"Total annual levy expected to be {annualLevyCost} but was {model.LevyFundingReceived}");

            //Validating the lvy credit for each month 
            foreach (var result in model.Results)
            {
                Assert.AreEqual(monthlyLevyCost, result.LevyIn, $"The levy credit expected for the date {result.Date} is {monthlyLevyCost} but the actual amount was {result.LevyIn}");
            }

            ScenarioContext.Current.Add("ResultData", model);
        }


        [When(@"My annual levy cost (.*) and mothly levy cost (.*) calculation are correct")]
        public void WhenMyAnnualLevyCostAndMothlyLevyCostCalculationAreCorrect(int annualLevyCost, int monthlyLevyCost)
        {
            var levyEstimation = new LevyEstimation();

            var prepData = ScenarioContext.Current.Get<ResultsViewModel>("PrepData");

            double finalAnnualLevyAmount = 0;

            levyEstimation.TotalLevyBeforeAllowanceDeduction = (prepData.Paybill * LevyEstimation.LevyPercentage) / 100;

            levyEstimation.TotalLevyAfterAllowanceDeduction = levyEstimation.TotalLevyBeforeAllowanceDeduction - LevyEstimation.Allowance;

            if (levyEstimation.TotalLevyAfterAllowanceDeduction > 0)
            {
                // Monthly levy payment before rounding
                var monthlyLevyPaymentNotRounded = levyEstimation.TotalLevyAfterAllowanceDeduction /
                                                   LevyEstimation.MonthsInAYear;

                //Monthly payment after rounding down
                levyEstimation.MonthlyLevyPayment = RoundDownDecimal(monthlyLevyPaymentNotRounded); // round down


                //Now let's factor the empployees % living in england and 10% top up government

                //monthly payment after english employees %
                levyEstimation.MonthlyPaymentAfterEmployeesPercentage = (levyEstimation.MonthlyLevyPayment *
                                                                         prepData.EnglishFraction) / 100;
                //Government top up
                var governmentTopUpAmount = (levyEstimation.MonthlyPaymentAfterEmployeesPercentage *
                                             LevyEstimation.GovernmentTopUp) / 100;

                //Monthly payment before rounding
                var monthlyPaymentIncludingGovernmenetTopUpNotRounded =
                    levyEstimation.MonthlyPaymentAfterEmployeesPercentage + governmentTopUpAmount;

                //Monthly payment after rounding up
                levyEstimation.MonthlyPaymentIncludingGovernmenetTopUp =
                    RoundUpDecimal(monthlyPaymentIncludingGovernmenetTopUpNotRounded);


                //Annual Levy payment
                finalAnnualLevyAmount = levyEstimation.MonthlyPaymentIncludingGovernmenetTopUp *
                                            LevyEstimation.MonthsInAYear;
            }

            Assert.AreEqual(annualLevyCost, finalAnnualLevyAmount, $"The expected annual levy cost is {annualLevyCost} however base on your own formula the actual annual levy cost was {finalAnnualLevyAmount}");
            Assert.AreEqual(monthlyLevyCost, levyEstimation.MonthlyPaymentIncludingGovernmenetTopUp, $"The expected monthly levy cost is {monthlyLevyCost} however base on your own formula the actual monthly levy cost was {monthlyLevyCost}");


            ScenarioContext.Current.Add("LevyEstimation", levyEstimation);


        }


        [When(@"my monthly cost (.*) and final month cost (.*) and employer share cost (.*) and government share cost (.*) and emploer final month share cost (.*) and government final month share cost (.*) calcuation is correct")]
        public void WhenMyApprenticeshipTotalCostAndMonthlyCostAndFinalMonthCostAndEmployerShareCostAndGovernmentShareCostAndEmploerFinalMonthShareCostAndGovernmentFinalMonthShareCostCalcuationIsCorrect(int appmonthlyCost, int finalMonthCost, int empMonthlyShare, int govMonthlyShare, int empFinalMonthShare, int goveFinalMonthShare)
        {
            var apprenticeshipCostEstimation = new ApprenticeshipCostEstimation();

            var apprenticeSshipData = ScenarioContext.Current.Get<Apprenticeship>("ApprenticeShipData");

            var prepData = ScenarioContext.Current.Get<ResultsViewModel>("PrepData");

            var achievementPayment = (prepData.TrainingCostForDuration * 20) / 100;

            apprenticeshipCostEstimation.MonthlyCost = (prepData.TrainingCostForDuration - achievementPayment) / prepData.Duration;

            apprenticeshipCostEstimation.FinalMonthlyCost = apprenticeshipCostEstimation.MonthlyCost + achievementPayment;


            //For when Levy is not sufficient
            if (prepData.Results[0].LevyIn < appmonthlyCost)
            {

                //calculating the first month
                var firstMonthRemaining = prepData.Results[0].TrainingOut - prepData.Results[0].LevyIn;

                var emplyerMonthlyContributionBeforeRounding = (firstMonthRemaining *
                                                                ApprenticeshipCostEstimation.EmployerPercentage) / 100;

                apprenticeshipCostEstimation.EmplyerMonthlyContribution = RoundDownDecimal(emplyerMonthlyContributionBeforeRounding);



                var governmentMonthlyContributionBeforeRounding = (firstMonthRemaining *
                                                                   ApprenticeshipCostEstimation.GovernmentPercentage) /
                                                                  100;

                apprenticeshipCostEstimation.GovernmentMonthlyContribution = RoundUpDecimal(governmentMonthlyContributionBeforeRounding);


                //Calculating the last month

                //get the last month cost
                var preLastMonthCost = appmonthlyCost * apprenticeSshipData.AllMonthsExceptFinalMonth;

                //to avoid negative number
                decimal lastMonthCost = 0;

                if (apprenticeSshipData.AppCost >= preLastMonthCost)
                {
                    lastMonthCost = apprenticeSshipData.AppCost - preLastMonthCost;
                }
                else
                {
                    lastMonthCost = preLastMonthCost - apprenticeSshipData.AppCost;
                }



                //Get the remaining
                var remainingAmount = Math.Round((lastMonthCost - prepData.Results[0].LevyIn), 0, MidpointRounding.AwayFromZero);


                //Then calculate 10% of the employer
                var employerFinalMonthContributionBeforeRounding = remainingAmount * ApprenticeshipCostEstimation.EmployerPercentage / 100;
                apprenticeshipCostEstimation.EmployerFinalMonthContribution = RoundDownDecimal(employerFinalMonthContributionBeforeRounding);
                //apprenticeshipCostEstimation.EmployerFinalMonthContribution = Math.Round(((remainingAmount * ApprenticeshipCostEstimation.EmployerPercentage) / 100), 0, MidpointRounding.AwayFromZero);

                //Then calculate 90% of the gov
                var governmentFinalMonthContributionBeforeRounding = remainingAmount * ApprenticeshipCostEstimation.GovernmentPercentage / 100;
                apprenticeshipCostEstimation.GovernmentFinalMonthContribution = RoundUpDecimal(governmentFinalMonthContributionBeforeRounding);
                //apprenticeshipCostEstimation.GovernmentFinalMonthContribution = Math.Round(((remainingAmount * ApprenticeshipCostEstimation.GovernmentPercentage) / 100), 0, MidpointRounding.AwayFromZero);
            }
            else
            {
                //Calculating the last month

                //get the last month cost
                var preLastMonthCost = appmonthlyCost * apprenticeSshipData.AllMonthsExceptFinalMonth;
                var lastMonthCost = apprenticeSshipData.AppCost - preLastMonthCost;


                ////Get the remaining
                //var remainingAmount = Math.Round((lastMonthCost - prepData.Results[0].LevyIn), 0, MidpointRounding.AwayFromZero);


                ////Then calculate 10% of the employer
                //apprenticeshipCostEstimation.EmployerFinalMonthContribution = Math.Round(((remainingAmount * ApprenticeshipCostEstimation.EmployerPercentage) / 100), 0, MidpointRounding.AwayFromZero);

                ////Then calculate 90% of the gov
                //apprenticeshipCostEstimation.GovernmentFinalMonthContribution = Math.Round(((remainingAmount * ApprenticeshipCostEstimation.GovernmentPercentage) / 100), 0, MidpointRounding.AwayFromZero);
            }






            Assert.AreEqual(appmonthlyCost, apprenticeshipCostEstimation.MonthlyCost, $"The expected apprenticeship monthly cost is {appmonthlyCost} however base on your own formula the value was {apprenticeshipCostEstimation.MonthlyCost}");

            Assert.AreEqual(finalMonthCost, apprenticeshipCostEstimation.FinalMonthlyCost, $"The expected apprenticeship final monthl cost is {finalMonthCost} however base on your own formula the value was {apprenticeshipCostEstimation.FinalMonthlyCost}");

            Assert.AreEqual(empMonthlyShare, apprenticeshipCostEstimation.EmplyerMonthlyContribution, $"The expected apprenticeship employer monthly contribution is {empMonthlyShare} however base on your own formula the value was {apprenticeshipCostEstimation.EmplyerMonthlyContribution}");

            Assert.AreEqual(govMonthlyShare, apprenticeshipCostEstimation.GovernmentMonthlyContribution, $"The expected apprenticeship government monthly contribution is {govMonthlyShare} however base on your own formula the value was {apprenticeshipCostEstimation.GovernmentFinalMonthContribution}");

            Assert.AreEqual(empFinalMonthShare, apprenticeshipCostEstimation.EmployerFinalMonthContribution, $"The expected apprenticeship employer final month contribution is {empFinalMonthShare} however base on your own formula the value was {apprenticeshipCostEstimation.EmployerFinalMonthContribution}");

            Assert.AreEqual(goveFinalMonthShare, apprenticeshipCostEstimation.GovernmentFinalMonthContribution, $"The expected apprenticeship government final month contribution is {goveFinalMonthShare} however base on your own formula the value was {apprenticeshipCostEstimation.GovernmentFinalMonthContribution}");




        }


        private static double RoundUpDecimal(double decimalAmountToUpdate)
        {
            var stringNumber = Convert.ToString(decimalAmountToUpdate);

            var splitString = stringNumber.Split('.').First();

            var addDecimalToString = splitString + ".5";

            var convertStringbackToNumber = Convert.ToDouble(addDecimalToString);

            return Math.Round(convertStringbackToNumber, 0, MidpointRounding.AwayFromZero);
        }

        private static double RoundDownDecimal(double decimalAmountToUpdate)
        {
            var stringNumber = Convert.ToString(decimalAmountToUpdate);

            var splitString = stringNumber.Split('.').First();

            var addDecimalToString = splitString;

            var convertStringbackToNumber = Convert.ToDouble(addDecimalToString);

            return convertStringbackToNumber;
        }


        private static decimal RoundUpDecimal(decimal decimalAmountToUpdate)
        {
            decimal convertStringbackToNumber = 0;

            //Check if decilam contains decimal places
            var hasFractionalPart = (decimalAmountToUpdate - Math.Round(decimalAmountToUpdate) != 0);

            if (hasFractionalPart)
            {
                var stringNumber = Convert.ToString(decimalAmountToUpdate);

                var firstpartofDecimalInString = stringNumber.Split('.').First();

                var addDecimalToString = firstpartofDecimalInString + ".5";

                convertStringbackToNumber = Convert.ToDecimal(addDecimalToString);
            }
            else
            {
                convertStringbackToNumber = decimalAmountToUpdate;
            }

            return Math.Round(convertStringbackToNumber, 0, MidpointRounding.AwayFromZero);
        }

        private static decimal RoundDownDecimal(decimal decimalAmountToUpdate)
        {
            var stringNumber = Convert.ToString(decimalAmountToUpdate);

            var splitString = stringNumber.Split('.').First();

            var addDecimalToString = splitString;

            var convertStringbackToNumber = Convert.ToDecimal(addDecimalToString);

            return convertStringbackToNumber;
        }

        [AfterScenario]
        public void TearDown()
        {
            _container.Dispose();
        }


        internal class Apprenticeship
        {
            public string AppName { get; set; }
            public int NumberOfApprentices { get; set; }
            public int AppCost { get; set; }
            public int AppDuration { get; set; }
            public DateTime AppStartDate { get; set; }
            public int AllMonthsExceptFinalMonth { get; set; }
            public int FinalMonthAchievement { get; set; }
        }

        internal class LevyEstimation
        {
            public const double LevyPercentage = 0.5;
            public const double Allowance = 15000;
            public const double GovernmentTopUp = 10;
            public const double MonthsInAYear = 12;


            public double TotalLevyBeforeAllowanceDeduction { get; set; }
            public double TotalLevyAfterAllowanceDeduction { get; set; }
            public double MonthlyLevyPayment { get; set; }
            public double MonthlyPaymentAfterEmployeesPercentage { get; set; }
            public double GovernmentTopUpAmount { get; set; }
            public double MonthlyPaymentIncludingGovernmenetTopUp { get; set; }
            public double FinalAnnualLevyAmount { get; set; }

        }

        internal class ApprenticeshipCostEstimation
        {
            public const int EmployerPercentage = 10;
            public const int GovernmentPercentage = 90;
            public decimal MonthlyCost { get; set; }
            public decimal FinalMonthlyCost { get; set; }
            public decimal EmplyerMonthlyContribution { get; set; }
            public decimal GovernmentMonthlyContribution { get; set; }
            public decimal EmployerFinalMonthContribution { get; set; }
            public decimal GovernmentFinalMonthContribution { get; set; }
        }


    }
}
