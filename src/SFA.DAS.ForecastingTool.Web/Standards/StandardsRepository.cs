using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Infrastructure.Services;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public class StandardsRepository : IStandardsRepository
    {
        private readonly IGetStandards _getStandards;

        public StandardsRepository(IGetStandards getStandards)
        {
            _getStandards = getStandards;
        }

        public async Task<Standard[]> GetAllAsync()
        {
            return _getStandards.GetAll();
        }

        public async Task<Standard> GetByCodeAsync(string code)
        {
            return _getStandards.GetByCode(code);
        }
    }
}