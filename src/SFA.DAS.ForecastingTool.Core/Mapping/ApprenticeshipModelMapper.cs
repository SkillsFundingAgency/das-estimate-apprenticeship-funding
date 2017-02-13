using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Core.Mapping
{
    public class ApprenticeshipModelMapper : IApprenticeshipModelMapper
    {
        public ApprenticeshipModel MapApprenticeshipModel(Apprenticeship apprenticeship)
        {
            return new ApprenticeshipModel
            {
                Code = apprenticeship.Code,
                Name = apprenticeship.Name,
                Price = apprenticeship.Price,
                Duration = apprenticeship.Duration
            };
        }
    }
}
