using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Infrastructure.Services
{
    public class GetApprenticeship : IGetApprenticeship
    {
        private readonly IStandardApiClient _standardApiClient;
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;

        public GetApprenticeship(IStandardApiClient standardApiClient, IFrameworkApiClient frameworkApiClient, IApprenticeshipMapper apprenticeshipMapper)
        {
            _standardApiClient = standardApiClient;
            _frameworkApiClient = frameworkApiClient;
            _apprenticeshipMapper = apprenticeshipMapper;
        }

        public Apprenticeship[] GetAll()
        {
            var standards = _standardApiClient.FindAll().ToList();
            var frameworks = _frameworkApiClient.FindAll().ToList();

            var result = standards.Select(standardSummary => _apprenticeshipMapper.MapStandardToApprenticeship(standardSummary)).ToList();
            result.AddRange(frameworks.Select(frameworkSummary => _apprenticeshipMapper.MapFrameworkToApprenticeship(frameworkSummary)));

            return result.ToArray();
        }

        public Apprenticeship GetByCode(string code)
        {
            var standards = _standardApiClient.FindAll().ToList();
            var frameworks = _frameworkApiClient.FindAll().ToList();

            var result = standards.Select(standardSummary => _apprenticeshipMapper.MapStandardToApprenticeship(standardSummary)).ToList();
            result.AddRange(frameworks.Select(frameworkSummary => _apprenticeshipMapper.MapFrameworkToApprenticeship(frameworkSummary)));

            return result.SingleOrDefault(x => x.Code == code);
        }
    }
}
