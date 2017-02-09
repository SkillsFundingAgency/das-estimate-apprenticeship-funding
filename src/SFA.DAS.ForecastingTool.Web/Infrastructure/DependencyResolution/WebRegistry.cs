using SFA.DAS.ForecastingTool.Infrastructure.Services;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
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
            container.Register<ICalculatorSettings, CalculatorSettings>(Lifestyle.Singleton);
            container.Register<IFileSystem, DiskFileSystem>(Lifestyle.Singleton);
            container.Register<IGetApprenticeship, GetApprenticeship>(Lifestyle.Singleton);

            container.Register<IApprenticeshipRepository>(() =>
            {
                var cacheProvider = container.GetInstance<ICacheProvider>();
                var innerRepo = container.GetInstance<ApprenticeshipRepository>();
                return new CachedApprenticeshipRepository(innerRepo, cacheProvider);
            }, Lifestyle.Singleton);

            container.Register<IForecastCalculator, ForecastCalculator>(Lifestyle.Singleton);

            return container;
        }
    }
}