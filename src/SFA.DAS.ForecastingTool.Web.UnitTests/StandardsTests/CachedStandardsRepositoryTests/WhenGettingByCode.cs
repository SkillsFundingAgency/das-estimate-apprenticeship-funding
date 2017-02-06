using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.CachedStandardsRepositoryTests
{
    public class WhenGettingByCode : CachedStandardsRepositoryTestsBase
    {
        [Test]
        public async Task ThenItShouldReturnCorrectStandard()
        {
            // Act
            var actual = await _repo.GetByCodeAsync("10");

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual("10", actual.Code);
            Assert.AreEqual("Inner 1", actual.Name);
            Assert.AreEqual(10000, actual.Price);
            Assert.AreEqual(12, actual.Duration);
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
