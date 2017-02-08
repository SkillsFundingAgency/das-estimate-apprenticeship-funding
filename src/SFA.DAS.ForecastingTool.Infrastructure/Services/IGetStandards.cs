using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Infrastructure.Services
{
    public interface IGetStandards
    {
        Apprenticeship[] GetAll();
        Apprenticeship GetByCode(string code);
    }
}