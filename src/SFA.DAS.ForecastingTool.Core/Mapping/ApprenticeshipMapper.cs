using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Core.Mapping
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        public Apprenticeship MapStandardToApprenticeship(StandardSummary standardSummary)
        {
            return new Apprenticeship
            {
                Code = standardSummary.Id,
                Duration = standardSummary.Duration,
                Price = standardSummary.MaxFunding,
                Name = $"{standardSummary.Title} - Level {standardSummary.Level}"
            };
        }

        public Apprenticeship MapFrameworkToApprenticeship(FrameworkSummary frameworkSummary)
        {
            return new Apprenticeship
            {
                Code = frameworkSummary.Id,
                Duration = frameworkSummary.Duration,
                Price = frameworkSummary.MaxFunding,
                Name = $"{frameworkSummary.Title} - Level {frameworkSummary.Level}"
            };
        }
    }
}
