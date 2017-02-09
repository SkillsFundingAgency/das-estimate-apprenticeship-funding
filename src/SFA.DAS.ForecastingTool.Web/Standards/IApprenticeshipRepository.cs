using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public interface IApprenticeshipRepository
    {
        Task<Apprenticeship[]> GetAllAsync();
        Task<Apprenticeship> GetByCodeAsync(string code);
    }
}