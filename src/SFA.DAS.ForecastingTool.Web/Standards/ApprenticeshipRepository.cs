using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly IStandardApiClient _standardApiClient;
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;

        public ApprenticeshipRepository(IStandardApiClient standardApiClient, IFrameworkApiClient frameworkApiClient, IApprenticeshipMapper apprenticeshipMapper)
        {
            _standardApiClient = standardApiClient;
            _frameworkApiClient = frameworkApiClient;
            _apprenticeshipMapper = apprenticeshipMapper;
        }

        public async Task<Apprenticeship[]> GetAllAsync()
        {
            var standards = _standardApiClient.FindAll();
            var frameworks = _frameworkApiClient.FindAll();

            var result = standards.Select(standardSummary => _apprenticeshipMapper.MapStandardToApprenticeship(standardSummary)).ToList();
            result.AddRange(frameworks.Select(frameworkSummary => _apprenticeshipMapper.MapFrameworkToApprenticeship(frameworkSummary)));

            return result.ToArray();
        }

        public async Task<Apprenticeship> GetByCodeAsync(string code)
        {
            var standards = _standardApiClient.FindAll();
            var frameworks = _frameworkApiClient.FindAll();

            var result = standards.Select(standardSummary => _apprenticeshipMapper.MapStandardToApprenticeship(standardSummary)).ToList();
            result.AddRange(frameworks.Select(frameworkSummary => _apprenticeshipMapper.MapFrameworkToApprenticeship(frameworkSummary)));

            return result.SingleOrDefault(x => x.Code == code);
        }
    }
}