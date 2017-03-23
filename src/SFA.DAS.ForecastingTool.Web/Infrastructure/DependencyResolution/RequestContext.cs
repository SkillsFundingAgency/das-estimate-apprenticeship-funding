using System.Web;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.DependencyResolution
{
    public sealed class RequestContext : IRequestContext
    {
        public RequestContext(HttpContextBase context)
        {
            try
            {
                IpAddress = context?.Request.UserHostAddress;
                Url = context?.Request.RawUrl;
            }
            catch (HttpException)
            {
                // Happens on request that starts the application.
                IpAddress = string.Empty;
                Url = string.Empty;
            }
        }

        public string IpAddress { get; private set; }

        public string Url { get; private set; }
    }
}