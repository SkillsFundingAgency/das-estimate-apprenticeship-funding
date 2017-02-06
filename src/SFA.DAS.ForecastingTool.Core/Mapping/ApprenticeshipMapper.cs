using SFA.DAS.Apprenticeships.Api.Types;
using Standard = SFA.DAS.ForecastingTool.Core.Models.Standard;

namespace SFA.DAS.ForecastingTool.Core.Mapping
{
    public class ApprenticeshipMapper
    {
        public Standard MapStandardToApprenticeship(StandardSummary standardSummary)
        {
            return new Standard
            {
                Code = standardSummary.Id,
                Duration = standardSummary.Duration,
                Price = standardSummary.MaxFunding,
                Name = standardSummary.Title
            };
        }

        public Standard MapFrameworkToApprenticeship(FrameworkSummary frameworkSummary)
        {
            return new Standard
            {
                Code = frameworkSummary.Id,
                Duration = frameworkSummary.Duration,
                Price = frameworkSummary.MaxFunding,
                Name = frameworkSummary.Title
            };
        }
    }
}
