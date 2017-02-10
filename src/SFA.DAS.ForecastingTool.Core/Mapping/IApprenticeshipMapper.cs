using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Core.Mapping
{
    public interface IApprenticeshipMapper
    {
        Apprenticeship MapFrameworkToApprenticeship(FrameworkSummary frameworkSummary);
        Apprenticeship MapStandardToApprenticeship(StandardSummary standardSummary);
    }
}