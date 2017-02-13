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
        protected Mock<IApprenticeshipRepository> _innerRepo;
        protected Mock<ICacheProvider> _cacheProvider;
        protected CachedApprenticeshipRepository _repo;

        [SetUp]
        public void Arrange()
        {
            _innerRepo = new Mock<IApprenticeshipRepository>();
            _innerRepo.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(new[]
            {
                new Apprenticeship {Code = "10", Name = "Inner 1", Price = 10000, Duration = 12}
            }));

            _cacheProvider = new Mock<ICacheProvider>();
            _cacheProvider.Setup(c => c.Get<Apprenticeship[]>(CacheKeys.Apprenticeships)).Returns<Apprenticeship[]>(null);

            _repo = new CachedApprenticeshipRepository(_innerRepo.Object, _cacheProvider.Object);
        }
    }
}
