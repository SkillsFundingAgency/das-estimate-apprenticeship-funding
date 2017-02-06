using System.Collections.Generic;
using System.IO;
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
    public class WhenGettingByCode
    {
        private Mock<IFileInfo> _standardsFileInfo;
        private Mock<IFileSystem> _fileSystem;
        private StandardsRepository _repo;
        private Mock<IGetStandards> _getStandard;

        [SetUp]
        public void Arrange()
        {
            var firstStandard = new Standard
            {
                Code = "11",
                Name = "Standard 11",
                Price = 24000,
                Duration = 12
            };
            var secondStandard = new Standard
            {
                Code = "22",
                Name = "Standard 22",
                Price = 12000,
                Duration = 6
            };

            _getStandard = new Mock<IGetStandards>();
            _getStandard.Setup(x => x.GetByCode("11")).Returns(firstStandard);
            _getStandard.Setup(x => x.GetByCode("22")).Returns(secondStandard);

            _repo = new StandardsRepository(_getStandard.Object);
        }

        [TestCase("11", "Standard 11", 24000, 12)]
        [TestCase("22", "Standard 22", 12000, 6)]
        public async Task ThenItShouldReturnCorrectStandard(string code, string name, int price, int duration)
        {
            // Act
            var actual = await _repo.GetByCodeAsync(code);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(code, actual.Code);
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(price, actual.Price);
            Assert.AreEqual(duration, actual.Duration);
        }

        [Test]
        public async Task ThenItShouldReturnNullIfNoStandardWithCode()
        {
            // Act
            var actual = await _repo.GetByCodeAsync("33");

            // Assert
            Assert.IsNull(actual);
        }
    }
}
