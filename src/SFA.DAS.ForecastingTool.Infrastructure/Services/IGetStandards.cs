using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Infrastructure.Services
{
    public interface IGetStandards
    {
        Standard[] GetAll();
        Standard GetByCode(string code);
    }
}