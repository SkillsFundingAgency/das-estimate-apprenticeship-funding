using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.StandardsRepositoryTests
{
    public class WhenGettingAll
    {
        private Mock<IFileInfo> _standardsFileInfo;
        private Mock<IFileSystem> _fileSystem;
        private StandardsRepository _repo;

        [SetUp]
        public void Arrange()
        {
            var data = "[{\"Code\":11,\"Name\":\"Standard 11\",\"Price\":24000,\"Duration\":12},{\"Code\":22,\"Name\":\"Standard 22\",\"Price\":12000,\"Duration\":6}]";
            _standardsFileInfo = new Mock<IFileInfo>();
            _standardsFileInfo.Setup(i => i.Exists).Returns(true);
            _standardsFileInfo.Setup(i => i.OpenRead(It.IsAny<FileShare>())).Returns(new MemoryStream(Encoding.UTF8.GetBytes(data)));

            _fileSystem = new Mock<IFileSystem>();
            _fileSystem.Setup(fs => fs.GetFile("~/App_Data/Standards.json"))
                .Returns(_standardsFileInfo.Object);

            _repo = new StandardsRepository(_fileSystem.Object);
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
            Assert.IsNotNull(expectedItem1, "Standard 11 is not in result");
            Assert.AreEqual("Standard 11", expectedItem1.Name);
            Assert.AreEqual(12, expectedItem1.Duration);
            Assert.AreEqual(24000, expectedItem1.Price);

            var expectedItem2 = actual.SingleOrDefault(s => s.Code == "22");
            Assert.IsNotNull(expectedItem2, "Standard 11 is not in result");
            Assert.AreEqual("Standard 22", expectedItem2.Name);
            Assert.AreEqual(6, expectedItem2.Duration);
            Assert.AreEqual(12000, expectedItem2.Price);
        }

        [Test]
        public void ThenItShouldThrowFileNotFoundExceptionIfFileInfoExistsIsFalse()
        {
            // Arrange
            _standardsFileInfo.Setup(i => i.Exists).Returns(false);

            // Act + Assert
            var ex = Assert.ThrowsAsync<FileNotFoundException>(async () => await _repo.GetAllAsync());
            Assert.AreEqual("Could not find Standards file", ex.Message);
        }

        [Test]
        public void ThenItShouldThrowInvalidDataExceptionIfFileContentIsCorrupt()
        {
            // Arrange
            _standardsFileInfo.Setup(i => i.OpenRead(It.IsAny<FileShare>())).Returns(new MemoryStream(Encoding.UTF8.GetBytes("<Notjson>")));

            // Act + Assert
            var ex = Assert.ThrowsAsync<InvalidDataException>(async () => await _repo.GetAllAsync());
            Assert.AreEqual("Standards data is corrupt", ex.Message);
            Assert.IsInstanceOf<JsonReaderException>(ex.InnerException);
        }
    }
}
