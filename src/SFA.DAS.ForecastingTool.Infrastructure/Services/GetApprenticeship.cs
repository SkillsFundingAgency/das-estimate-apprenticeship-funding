using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Infrastructure.Services
{
    public class GetApprenticeship : IGetApprenticeship
    {
        readonly ApprenticeshipMapper _mapper;

        readonly StandardApiClient _standardClient;
        readonly FrameworkApiClient _frameworkClient;

        public GetApprenticeship()
        {
            _mapper = new ApprenticeshipMapper();

            _standardClient = new StandardApiClient();
            _frameworkClient = new FrameworkApiClient();
        }

        public Apprenticeship[] GetAll()
        {
            var standards = _standardClient.FindAll().ToList();
            var frameworks = _frameworkClient.FindAll().ToList();

            var result = standards.Select(standardSummary => _mapper.MapStandardToApprenticeship(standardSummary)).ToList();
            result.AddRange(frameworks.Select(frameworkSummary => _mapper.MapFrameworkToApprenticeship(frameworkSummary)));

            return result.ToArray();
        }

        public Apprenticeship GetByCode(string code)
        {
            var standards = _standardClient.FindAll().ToList();
            var frameworks = _frameworkClient.FindAll().ToList();

            var result = standards.Select(standardSummary => _mapper.MapStandardToApprenticeship(standardSummary)).ToList();
            result.AddRange(frameworks.Select(frameworkSummary => _mapper.MapFrameworkToApprenticeship(frameworkSummary)));

            return result.SingleOrDefault(x => x.Code == code);
        }
    }
}
