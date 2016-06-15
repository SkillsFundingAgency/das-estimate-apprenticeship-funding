using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;

namespace SFA.DAS.ForecastingTool.Web.Standards
{
    public class StandardsRepository : IStandardsRepository
    {
        private readonly IFileSystem _fileSystem;

        public StandardsRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task<Standard[]> GetAllAsync()
        {
            var standardsFile = _fileSystem.GetFile("~/App_Data/Standards.json");
            if (!standardsFile.Exists)
            {
                throw new FileNotFoundException("Could not find Standards file");
            }

            using (var stream = standardsFile.OpenRead(FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                try
                {
                    var json = await reader.ReadToEndAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<Standard[]>(json);
                    }
                    catch (JsonReaderException ex)
                    {
                        throw new InvalidDataException("Standards data is corrupt", ex);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        public async Task<Standard> GetByCodeAsync(int code)
        {
            var standards = await GetAllAsync();
            return standards.SingleOrDefault(s => s.Code == code);
        }
    }
}