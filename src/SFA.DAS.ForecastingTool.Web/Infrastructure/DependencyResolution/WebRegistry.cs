using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Configuration;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Standards;
using SimpleInjector;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.DependencyResolution
{
    public class WebRegistry
    {
        public Container Build()
        {
            var container = new Container();

            container.Register<ICacheProvider, InProcessCacheProvider>(Lifestyle.Singleton);
            container.Register<IConfigurationProvider, AppSettingsConfigurationProvider>(Lifestyle.Singleton);
            container.Register<IFileSystem, DiskFileSystem>(Lifestyle.Singleton);

            container.Register<IStandardsRepository>(() =>
            {
                var cacheProvider = container.GetInstance<ICacheProvider>();
                var innerRepo = container.GetInstance<StandardsRepository>();
                return new CachedStandardsRepository(innerRepo, cacheProvider);
            }, Lifestyle.Singleton);

            container.Register<IForecastCalculator, ForecastCalculator>(Lifestyle.Singleton);

            return container;
        }
    }
}