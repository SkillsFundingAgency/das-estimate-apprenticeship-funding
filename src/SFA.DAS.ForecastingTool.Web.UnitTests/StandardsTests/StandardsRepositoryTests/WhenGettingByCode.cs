using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.StandardsRepositoryTests
{
    public class WhenGettingByCode
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

        [Test]
        public void ThenItShouldThrowFileNotFoundExceptionIfFileInfoExistsIsFalse()
        {
            // Arrange
            _standardsFileInfo.Setup(i => i.Exists).Returns(false);

            // Act + Assert
            var ex = Assert.ThrowsAsync<FileNotFoundException>(async () => await _repo.GetByCodeAsync("11"));
            Assert.AreEqual("Could not find Standards file", ex.Message);
        }

        [Test]
        public void ThenItShouldThrowInvalidDataExceptionIfFileContentIsCorrupt()
        {
            // Arrange
            _standardsFileInfo.Setup(i => i.OpenRead(It.IsAny<FileShare>())).Returns(new MemoryStream(Encoding.UTF8.GetBytes("<Notjson>")));

            // Act + Assert
            var ex = Assert.ThrowsAsync<InvalidDataException>(async () => await _repo.GetByCodeAsync("11"));
            Assert.AreEqual("Standards data is corrupt", ex.Message);
            Assert.IsInstanceOf<JsonReaderException>(ex.InnerException);
        }
    }
}
