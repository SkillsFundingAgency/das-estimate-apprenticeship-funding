using System.Threading.Tasks;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public interface IStandardsRepository
    {
        Task<Apprenticeship[]> GetAllAsync();
        Task<Apprenticeship> GetByCodeAsync(string code);
    }
}