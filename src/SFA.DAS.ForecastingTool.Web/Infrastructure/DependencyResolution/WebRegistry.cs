using System;
using System.Web;
using System;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ForecastingTool.Core.Mapping;
using SFA.DAS.ForecastingTool.Web.FinancialForecasting;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Caching;
using SFA.DAS.ForecastingTool.Web.Infrastructure.FileSystem;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
using SFA.DAS.ForecastingTool.Web.Standards;
using SFA.DAS.NLog.Logger;
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
            container.Register<ILog>(() => new NLogLogger(GetType(), new RequestContext(new HttpContextWrapper(HttpContext.Current))), Lifestyle.Singleton);

            container.Register<IApprenticeshipRepository>(() =>
            {
                var cacheProvider = container.GetInstance<ICacheProvider>();
                var innerRepo = container.GetInstance<ApprenticeshipRepository>();
                return new CachedApprenticeshipRepository(innerRepo, cacheProvider);
            }, Lifestyle.Singleton);

            container.Register<IForecastCalculator, ForecastCalculator>(Lifestyle.Singleton);

            container.Register<IStandardApiClient>(() => new StandardApiClient(), Lifestyle.Transient);
            container.Register<IFrameworkApiClient>(() => new FrameworkApiClient(), Lifestyle.Transient);
            container.Register<IApprenticeshipMapper, ApprenticeshipMapper>();
            container.Register<IApprenticeshipModelMapper, ApprenticeshipModelMapper>();

            return container;
        }
    }
}