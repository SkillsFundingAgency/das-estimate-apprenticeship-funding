using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Infrastructure.Services;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly IGetApprenticeship _getApprenticeship;

        public ApprenticeshipRepository(IGetApprenticeship getApprenticeship)
        {
            _getApprenticeship = getApprenticeship;
        }

        public async Task<Apprenticeship[]> GetAllAsync()
        {
            return _getApprenticeship.GetAll();
        }

        public async Task<Apprenticeship> GetByCodeAsync(string code)
        {
            return _getApprenticeship.GetByCode(code);
        }
    }
}