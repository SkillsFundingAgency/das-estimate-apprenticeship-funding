using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.ModelsTests.ForecastQuestionsModelTests
{
    public class WhenGeneratingTheCohortUrl
    {
        private ForecastQuestionsModel _model;

        [SetUp]
        public void Arrange()
        {
            _model = new ForecastQuestionsModel();
        }

        [Test]
        public void ThenTheDateIsFormattedCorrectlyForSingleCohorts()
        {
            //Arrange
            _model.SelectedCohorts = new List<CohortModel> {new CohortModel
            {
                Code=1,
                Name ="test",
                Qty =4,
                StartDate = new DateTime(2018,05,03)
            } };
            
            //Act
            var actual = _model.GetCohortsUrl();

            //Assert
            Assert.AreEqual("4x1-0518", actual);
        }


        [Test]
        public void ThenTheDateIsFormattedCorrectlyForMultipleCohorts()
        {
            //Arrange
            _model.SelectedCohorts = new List<CohortModel>
            {
                new CohortModel
                {
                    Code=1,
                    Name ="test",
                    Qty =4,
                    StartDate = new DateTime(2018,05,03)
                },
                new CohortModel
                {
                    Code=2,
                    Name ="test2",
                    Qty =5,
                    StartDate = new DateTime(2019,10,03)
                }
            };

            //Act
            var actual = _model.GetCohortsUrl();

            //Assert
            Assert.AreEqual("4x1-0518_5x2-1019", actual);
        }
    }
}
