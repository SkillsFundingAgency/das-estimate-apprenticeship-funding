using System.Collections.Generic;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class ParsedUrl
    {
        public string ActionName { get; set; }
        public Dictionary<string, object> RouteValues { get; set; }
    }
}