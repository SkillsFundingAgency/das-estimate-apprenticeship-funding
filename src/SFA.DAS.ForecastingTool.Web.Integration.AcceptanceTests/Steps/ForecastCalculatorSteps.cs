using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Controllers;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Models;
using SFA.DAS.ForecastingTool.Web.Standards;
using SimpleInjector;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.ForecastingTool.Web.Integration.AcceptanceTests.Steps
{
    [Binding]
    public class ForecastCalculatorSteps
    {
        private Container _container;
        private HomeController _homeController;
        private ResultsViewModel _resultsViewModel;
        private IStandardsRepository _standardsRepository;

        [BeforeScenario]
        public void Arrange()
        {
            _container = new Infrastructure.DependencyResolution.ContainerBuilder().Build();

            _standardsRepository = _container.GetInstance<IStandardsRepository>();

            _homeController = new HomeController(_standardsRepository, _container.GetInstance<IForecastCalculator>(), _container.GetInstance<IConfigurationProvider>());

            _resultsViewModel = new ResultsViewModel();
        }

        [When(@"I Have the following apprenticeships:")]
        public void GivenIHaveTheFollowingApprenticeships(Table apprenticeshipTable)
        {
            var apprenticeships = apprenticeshipTable.CreateSet<Apprenticeship>();

            foreach (var apprenticeship in apprenticeships)
            {
                var standard = _standardsRepository.GetByName(apprenticeship.Name).Result;

                _resultsViewModel.SelectedCohorts.Add(new CohortModel
                {
                    Code = standard.Code,
                    StartDate = Convert.ToDateTime(apprenticeship.StartDate),
                    Name = apprenticeship.Name,
                    Qty = apprenticeship.Qty
                });
            }
        }


        [Given(@"I have a paybill of (.*) and my English Fraction is (.*)")]
        public void GivenIHaveAPaybillOfAndMyEnglishFractionIs(int paybill, int englishFraction)
        {
            _resultsViewModel.Duration = 36;
            _resultsViewModel.EnglishFraction = englishFraction;
            _resultsViewModel.Paybill = paybill;
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
            Assert.AreEqual(totalCost, model.TrainingCostForDuration,$"Total cost expected to be {totalCost} but was {model.TrainingCostForDuration}");
            Assert.AreEqual(totalGovermentPays, model.Results.Sum(c => c.CoPaymentGovernment),$"Total Goverment Pays expected to be {totalGovermentPays} but was {model.Results.Sum(c => c.CoPaymentGovernment)}");
            Assert.AreEqual(totalEmployerContribution, model.Results.Sum(c=>c.CoPaymentEmployer), $"Total Employer Contribution expected to be {totalEmployerContribution} but was { model.Results.Sum(c => c.CoPaymentEmployer)}");
        }
        
        [AfterScenario]
        public void TearDown()
        {
            _container.Dispose();
        }


        internal class Apprenticeship
        {
            public string Name { get; set; }
            public int Qty { get; set; }
            public DateTime StartDate { get; set; }
        }
    }
}
