using System.Threading.Tasks;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public interface IStandardsRepository
    {
        Task<Standard[]> GetAllAsync();
        Task<Standard> GetByCodeAsync(int code);
    }
}