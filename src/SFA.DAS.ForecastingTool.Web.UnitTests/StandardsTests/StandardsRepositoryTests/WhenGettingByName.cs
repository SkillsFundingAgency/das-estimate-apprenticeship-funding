using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.StandardsRepositoryTests
{
    public class WhenGettingByName
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
        public async Task ThenTheItemIsRetrieved()
        {
            //Arrange
            var name = "Standard 22";

            //Act
            var actual = await _repo.GetByName(name);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenNullIsReturnedIfItDoesNotExist()
        {
            //Arrange
            var name = "standard x";

            //Act
            var actual = await _repo.GetByName(name);


            //Assert
            Assert.IsNull(actual);
        }
    }
}
