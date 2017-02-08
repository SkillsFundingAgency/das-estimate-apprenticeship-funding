using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.CachedStandardsRepositoryTests
{
    public class WhenGettingAll : CachedStandardsRepositoryTestsBase
    {
        [Test]
        public async Task ThenItShouldReturnCachedResultsIfPresent()
        {
            // Arrange
            _cacheProvider.Setup(c => c.Get<Apprenticeship[]>(CacheKeys.Standards)).Returns(new[]
            {
                new Apprenticeship {Code = "20", Name = "Cache 1", Price = 200000, Duration = 24}
            });

            // Act
            var actual = await _repo.GetAllAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("20", actual[0].Code);
            Assert.AreEqual("Cache 1", actual[0].Name);
            Assert.AreEqual(200000, actual[0].Price);
            Assert.AreEqual(24, actual[0].Duration);
        }

        [Test]
        public async Task ThenItShouldReturnDataFromInnerRepositoryIfNoDataCached()
        {
            // Act
            var actual = await _repo.GetAllAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("10", actual[0].Code);
            Assert.AreEqual("Inner 1", actual[0].Name);
            Assert.AreEqual(10000, actual[0].Price);
            Assert.AreEqual(12, actual[0].Duration);
        }

        [Test]
        public async Task ThenItShouldStoreDataFromInnerRepositoryInCacheIfItContainsRecords()
        {
            // Act
            await _repo.GetAllAsync();

            // Assert
            _cacheProvider.Verify(c=>c.Set(CacheKeys.Standards, It.IsAny<Apprenticeship[]>(), It.Is<TimeSpan>(ts => ts.Hours == 6)), Times.Once());
        }

        [Test]
        public async Task ThenItShouldNotStoreDataFromInnerRepositoryInCacheIfItDoesNotContainRecords()
        {
            // Arrange
            _innerRepo.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(new Apprenticeship[0]));

            // Act
            await _repo.GetAllAsync();

            // Assert
            _cacheProvider.Verify(c => c.Set(CacheKeys.Standards, It.IsAny<Apprenticeship[]>(), It.IsAny<TimeSpan>()), Times.Never());
            _cacheProvider.Verify(c => c.Set(CacheKeys.Standards, It.IsAny<Apprenticeship[]>(), It.IsAny<DateTimeOffset>()), Times.Never());
        }
    }
}
