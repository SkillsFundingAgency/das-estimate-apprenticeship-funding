using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.UnitTests.StandardsTests.CachedStandardsRepositoryTests
{
    public abstract class CachedStandardsRepositoryTestsBase
    {
        protected Mock<IStandardsRepository> _innerRepo;
        protected Mock<ICacheProvider> _cacheProvider;
        protected CachedStandardsRepository _repo;

        [SetUp]
        public void Arrange()
        {
            _innerRepo = new Mock<IStandardsRepository>();
            _innerRepo.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(new[]
            {
                new Standard {Code = "10", Name = "Inner 1", Price = 10000, Duration = 12}
            }));

            _cacheProvider = new Mock<ICacheProvider>();
            _cacheProvider.Setup(c => c.Get<Standard[]>(CacheKeys.Standards)).Returns<Standard[]>(null);

            _repo = new CachedStandardsRepository(_innerRepo.Object, _cacheProvider.Object);
        }
    }
}
