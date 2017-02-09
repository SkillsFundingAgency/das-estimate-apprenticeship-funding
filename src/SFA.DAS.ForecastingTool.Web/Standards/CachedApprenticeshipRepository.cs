using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public class CachedApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly IApprenticeshipRepository _innerRepository;
        private readonly ICacheProvider _cacheProvider;

        public CachedApprenticeshipRepository(IApprenticeshipRepository innerRepository, ICacheProvider cacheProvider)
        {
            _innerRepository = innerRepository;
            _cacheProvider = cacheProvider;
        }

        public async Task<Apprenticeship[]> GetAllAsync()
        {
            var apprenticeships = _cacheProvider.Get<Apprenticeship[]>(CacheKeys.Apprenticeships);
            if (apprenticeships == null)
            {
                apprenticeships = await _innerRepository.GetAllAsync();
                if (apprenticeships.Length > 0)
                {
                    _cacheProvider.Set(CacheKeys.Apprenticeships, apprenticeships, new TimeSpan(6, 0, 0));
                }
            }
            return apprenticeships;
        }

        public async Task<Apprenticeship> GetByCodeAsync(string code)
        {
            var standards = await GetAllAsync();
            return standards.SingleOrDefault(s => s.Code == code);
        }
    }
}