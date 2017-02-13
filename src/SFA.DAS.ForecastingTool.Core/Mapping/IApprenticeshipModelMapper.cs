using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Core.Mapping
{
    public interface IApprenticeshipModelMapper
    {
        ApprenticeshipModel MapApprenticeshipModel(Apprenticeship apprenticeship);
    }
}