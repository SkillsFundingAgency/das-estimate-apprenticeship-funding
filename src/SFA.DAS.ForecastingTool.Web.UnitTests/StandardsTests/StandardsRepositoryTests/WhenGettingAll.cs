using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Infrastructure.Services;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.StandardsRepositoryTests
{
    public class WhenGettingAll
    {
        private Mock<IFileInfo> _standardsFileInfo;
        private Mock<IFileSystem> _fileSystem;
        private StandardsRepository _repo;
        private Mock<IGetStandards> _getStandard;

        [SetUp]
        public void Arrange()
        {
            var data = new List<Apprenticeship>
            {
                new Apprenticeship
                {
                    Code = "11",
                    Name = "Apprenticeship 11",
                    Price = 24000,
                    Duration = 12
                },
                new Apprenticeship
                {
                    Code = "22",
                    Name = "Apprenticeship 22",
                    Price = 12000,
                    Duration = 6
                }
            };

            _getStandard = new Mock<IGetStandards>();
            _getStandard.Setup(x => x.GetAll()).Returns(data.ToArray);
            _repo = new StandardsRepository(_getStandard.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAnArrayOfStandards()
        {
            // Act
            var actual = await _repo.GetAllAsync();

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnAllStandardsStoredInFile()
        {
            // Act
            var actual = await _repo.GetAllAsync();

            // Assert
            Assert.AreEqual(2, actual.Length);

            var expectedItem1 = actual.SingleOrDefault(s => s.Code == "11");
            Assert.IsNotNull(expectedItem1, "Apprenticeship 11 is not in result");
            Assert.AreEqual("Apprenticeship 11", expectedItem1.Name);
            Assert.AreEqual(12, expectedItem1.Duration);
            Assert.AreEqual(24000, expectedItem1.Price);

            var expectedItem2 = actual.SingleOrDefault(s => s.Code == "22");
            Assert.IsNotNull(expectedItem2, "Apprenticeship 11 is not in result");
            Assert.AreEqual("Apprenticeship 22", expectedItem2.Name);
            Assert.AreEqual(6, expectedItem2.Duration);
            Assert.AreEqual(12000, expectedItem2.Price);
        }
    }
}
